using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class QueryTranslator : ExpressionVisitor
    {
        StringBuilder sb;
        ParameterExpression row;
        ColumnProjection projection;
        internal QueryTranslator()
        {

        }

        /// <summary>
        /// 将表达式转换为 TranslateResult
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal TranslateResult Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            // 对于 Select 投射表达式，例如 row => new { A=row.A, B=row.B, C=row.C }
            // 定义一个命名为 row 的 ParameterExpression
            this.row = Expression.Parameter(typeof(ProjectionRow), "row");
            this.Visit(expression);
            return new TranslateResult
            {
                // 解析出来的 SQL 命令字符串
                CommandText = this.sb.ToString(),
                // 如果为 Select 投射
                // 则创建一个 LambdaExpression
                //  参数为一个 ProjectionRow 对象
                //  body 为 Selector，返回一个匿名类型
                // 所以此 LambdaExpression 的签名符合 Func<ProjectionRow, T> 的形式
                Projector = this.projection != null ? Expression.Lambda(this.projection.Selector, this.row) : null
            };
        }

        /// <summary>
        /// 将 Expression<LambdaExpression> 转换为 LambdaExpression 形式
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        /// <summary>
        /// 方法调用时执行
        /// 目前支持 Where、Select 方法
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable))
            {
                if (m.Method.Name == "Where")
                {
                    sb.Append("SELECT * FROM (");
                    // 第一个参数是 Expression.Constant(this)，例如 Query<Customers>
                    // 此时将会调用 VisitConstant 方法
                    // 此时，将会执行 sb.Append("SELECT * FROM Customers");
                    this.Visit(m.Arguments[0]);
                    sb.Append(") AS T WHERE ");
                    // 解析出 x=> x.A=1 && x.B=2 LambdaExpression 表示的形式
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    this.Visit(lambda.Body);
                    return m;
                }
                else if (m.Method.Name == "Select")
                {
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    // 创建一个投射对象，包含两个属性
                    //   Columns 属性表示需要查询的列字符串，类似于 A,B,C,D
                    //   Selector 是一个表达式，通过 Expression.Lambda 方法转换后符合 Func<ProjectionRow, T> 的形式
                    ColumnProjection projection = new ColumnProjector().ProjectColumns(lambda.Body, this.row);
                    sb.Append("SELECT ");
                    sb.Append(projection.Columns);
                    sb.Append(" FROM (");
                    // 第一个参数
                    this.Visit(m.Arguments[0]);
                    sb.Append(") AS T ");
                    this.projection = projection;
                    return m;
                }
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        /// <summary>
        /// 将一元操作符 ! 转换为 NOT 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        /// <summary>
        /// 将二元操作符转换为相应的 SQL 关系符
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        /// <summary>
        /// 解析常量表达式
        /// 将其直接解析为 SQL 字面量
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                sb.Append("SELECT * FROM ");
                sb.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                    default:
                        sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        /// <summary>
        /// 将属性访问转换为对应的 SQL 字段形式
        /// 例如 a.City 转换为 "City"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(node.Member.Name);
                return node;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
        }
    }
}
