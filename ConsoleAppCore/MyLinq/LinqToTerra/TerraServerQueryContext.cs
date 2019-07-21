using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Collections;
using System.Linq;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    class TerraServerQueryContext
    {
        /// <summary>
        /// 执行 Execute 操作
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isEnumerable"></param>
        /// <returns></returns>
        internal static object Execute(Expression expression, bool isEnumerable)
        {
            if (!IsQueryOverDataSource(expression))
            {
                throw new InvalidProgramException("No query over the data source was specified.");
            }

            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();

            // 获取最内层的 Where 表达式
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);

            // Where 方法有两个参数，第一个是数据源，第二个是表达式
            // 此处获取第二个参数，表达式
            // 获取 Where 表达式里面的 LambdaExpression
            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;


            // Send the lambda expression through the partial evaluator.
            // 将变量表达式执行求值操作
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            // Get the place name(s) to query the Web service with.
            // 解析 Where 表达式体，获取里面的参数值，用于 WebService 查询
            LocationFinder lf = new LocationFinder(lambdaExpression.Body);
            List<string> locations = lf.Locations;
            if (locations.Count == 0)
                throw new InvalidQueryException("You must specify at least one place name in your query.");

            // Call the Web service and get the results.
            // 从 Where 中解析的参数，调用 WebService
            Place[] places = WebServiceHelper.GetPlacesFromTerraServer(locations);

            // Copy the IEnumerable places to an IQueryable.
            // 这个对象已经可以执行 Linq to Object 操作了
            IQueryable<Place> queryablePlaces = places.AsQueryable<Place>();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.

            // 将 IQueryable<Place> 存储在 treeCopier 中
            ExpressionTreeModifier treeCopier = new ExpressionTreeModifier(queryablePlaces);
            // 如果原始表达式是一个 QueryableTerraServerData<Place> 对象
            // 则返回一个类型是 IQueryable<Place> 的 ConstantExpression
            // 可以在这个对象上执行 Linq to Object 的查询
            Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.     
            // 将自定义的 Query 和 Provider 替换为 Linq to Object 的 Provider 和 IQueryable
            if (isEnumerable)
                // 切换到原生的 Linq to Object IQueryable
                return queryablePlaces.Provider.CreateQuery(newExpressionTree);
            else
                return queryablePlaces.Provider.Execute(newExpressionTree);

        }

        /// <summary>
        /// 判断表达式是否为 MethodCallExpression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }
    }
}
