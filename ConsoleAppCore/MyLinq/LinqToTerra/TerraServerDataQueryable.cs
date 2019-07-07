using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    class TerraServerDataQueryable<TData> : IOrderedQueryable<TData>
    {
        /// <summary>
        /// 这个构造函数没有参数
        /// </summary>
        public TerraServerDataQueryable()
        {
            // 创建对应的 IQueryProvider
            // 当执行 from it in source 语句时
            // 会调用 IQueryProvider.CreateQuery 方法
            Provider = new TerraServerQueryProvider();
            Expression = Expression.Constant(this);
        }

        /// <summary>
        /// 这个构造函数在 TerraServerQueryProvider 中调用
        /// 它在执行 CreateQuery 时，返回这个 IQuayrable
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>

        public TerraServerDataQueryable(TerraServerQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<TData>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
        }

        #region 这些都是需要实现的对象
        public Type ElementType => typeof(TData);

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public IEnumerator<TData> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<TData>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
        #endregion

    }
}
