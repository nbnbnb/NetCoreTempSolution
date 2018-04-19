using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class ColumnProjector : ExpressionVisitor
    {
        StringBuilder sb;
        int iColumn;
        ParameterExpression row;
        static MethodInfo miGetValue;

        internal ColumnProjector()
        {
            if (miGetValue == null)
            {
                miGetValue = typeof(ProjectionRow).GetMethod("GetValue");
            }
        }

        internal ColumnProjection ProjectColumns(Expression expression, ParameterExpression row)
        {
            this.sb = new StringBuilder();
            this.row = row;
            // 将 item.A 形式的属性调用，转换为 MethodCallExpression 方式的调用
            Expression selector = this.Visit(expression);
            return new ColumnProjection { Columns = this.sb.ToString(), Selector = selector };
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                if (this.sb.Length > 0)
                {
                    this.sb.Append(", ");
                }
                this.sb.Append(m.Member.Name);
                // 执行方法调用，并类型转换
                // 方法实例为一个 ProjectionRow 对象，参数是 index 
                // index 将在每次求值后自加
                return Expression.Convert(Expression.Call(this.row, miGetValue, Expression.Constant(iColumn++)), m.Type);
            }
            else
            {
                return base.VisitMember(m);
            }
        }
    }
}
