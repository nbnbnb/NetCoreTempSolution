using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    /// <summary>
    /// 将 QueryableTerraServerData<Place> 对象，转换为 IQueryable<Place>
    /// 这样就可以使用 Linq to Object 的功能了
    /// </summary>
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private IQueryable<Place> queryablePlaces;

        internal ExpressionTreeModifier(IQueryable<Place> places)
        {
            this.queryablePlaces = places;
        }

        /// <summary>
        /// 将 QueryableTerraServerData<Place> 对象，转换为 IQueryable<Place>
        /// 这样就可以使用 Linq to Object 的功能了
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
            if (c.Type == typeof(TerraServerDataQueryable<Place>))
                return Expression.Constant(this.queryablePlaces);
            else
                return c;
        }
    }
}
