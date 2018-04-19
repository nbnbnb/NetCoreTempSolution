using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    public class DbQueryProvider : QueryProvider
    {
        DbConnection connection;

        /// <summary>
        /// 构造一个 DbQueryProvider，传递一个 DbConnection
        /// </summary>
        /// <param name="connection"></param>
        public DbQueryProvider(DbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 将表达式转换为 SQL 命令字符串
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override string GetQueryText(Expression expression)
        {
            return this.Translate(expression).CommandText;
        }

        /// <summary>
        /// 执行表达式
        /// 根据表达式解析出的 SQLCommand，执行数据库查询
        /// 
        /// 将得到的 DbDataReader，构造成 ProjectionReader 和 ObjectReader
        /// 他们都是可枚举类型，可以从 DbDataReader 中一行行填充数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override object Execute(Expression expression)
        {
            TranslateResult result = this.Translate(expression);
            DbCommand cmd = this.connection.CreateCommand();
            cmd.CommandText = result.CommandText;
            DbDataReader reader = cmd.ExecuteReader();
            Type elementType = TypeSystem.GetElementType(expression.Type);

            // 判断是否有 Select 投射子句
            // 如果有，则创建 ProjectionReader
            // 没有则创建 ObjectReader
            if (result.Projector != null)
            {
                // Projector 是一个 Lambda 表达式
                //    body 是通过 ColumnProjector 转换后的一个 SelectExpression，它返回一个 T 对象
                //    参数为一个继承至 ProjectionRow 的对象

                // 所以 result.Projector 编译后，正好符合 Func<ProjectionRow, T> 的签名
                Delegate projector = result.Projector.Compile();

                return Activator.CreateInstance(
                    typeof(ProjectionReader<>).MakeGenericType(elementType),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { reader, projector },
                    null
                    );
            }
            else
            {
                return Activator.CreateInstance(
                    typeof(ObjectReader<>).MakeGenericType(elementType),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { reader },
                    null
                    );
            }
        }

        /// <summary>
        /// 转换表达式
        /// 如果有 Select 子句，则 TranslateResult.Projector 不为空
        /// 用于投射 Select 子句生成的对象
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new QueryTranslator().Translate(expression);
        }
    }
}
