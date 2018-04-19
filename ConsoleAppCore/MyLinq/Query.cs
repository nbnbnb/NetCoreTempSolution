using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        QueryProvider provider;
        Expression expression;

        public Query(QueryProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException("provider");
            // 初始化时，Expression 属性就是自身的一个 ConstantExpression
            expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            // 此处验证了传递的 Expression 是不是 IQueryable<T> 的子类型
            // 默认构造时，传递的 Expression 为通过自身构建的一个 ConstantExpression
            // 所以符合要求
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            this.provider = provider;
            this.expression = expression;
        }

        /// <summary>
        /// 返回查询对象类型
        /// </summary>
        public Type ElementType => typeof(T);

        /// <summary>
        /// 返回 Expression
        /// </summary>
        public Expression Expression => this.expression;

        /// <summary>
        /// 返回 Provider
        /// </summary>
        public IQueryProvider Provider => this.provider;

        /// <summary>
        /// 当调用 Provider 的 Execute 执行时
        /// 内部将会通过 SqlReader 迭代返回数据化，然后构造为一个 IEnumerable<T>
        /// 所以此处使用了强制类型转换，不会报错
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.provider.Execute(this.expression)).GetEnumerator();
        }

        /// <summary>
        /// 同上
        /// 非泛型
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        }

        /// <summary>
        /// 重写了 Query 对象的 ToString 方法
        /// 控制台输出的时候，将会对表达式进行解析
        /// 构建出 SQL 语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.provider.GetQueryText(this.expression);
        }
    }
}
