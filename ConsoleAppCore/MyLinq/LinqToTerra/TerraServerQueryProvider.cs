using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    /// <summary>
    /// 自定义 IQueryProvider
    /// </summary>
    class TerraServerQueryProvider : IQueryProvider
    {

        /// <summary>
        /// 非泛型的 IQueryable<T>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                // 返回一个 IQueryable 对象
                return (IQueryable)Activator.CreateInstance(
                    typeof(TerraServerDataQueryable<>).MakeGenericType(elementType),
                    new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// 泛型的 IQueryable<T>
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TerraServerDataQueryable<TElement>(this, expression);
        }

        /// <summary>
        /// 执行 ToList 的时候会调用
        /// 非泛型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object Execute(Expression expression)
        {
            return TerraServerQueryContext.Execute(expression, false);
        }

        /// <summary>
        /// 执行 ToList 的时候会调用
        /// 泛型
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)TerraServerQueryContext.Execute(expression, isEnumerable);
        }
    }
}
