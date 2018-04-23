using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    class TerraServerQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(
                    typeof(QueryableTerraServerData<>).MakeGenericType(elementType),
                    new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new QueryableTerraServerData<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return TerraServerQueryContext.Execute(expression, false);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)TerraServerQueryContext.Execute(expression, isEnumerable);
        }
    }
}
