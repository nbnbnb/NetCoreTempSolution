using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    /// <summary>
    /// 解析表达式中的二元操作符（Equals）
    /// 例如 Name="AAA" 和 State="BBB" 这种表达式
    /// </summary>
    class LocationFinder : ExpressionVisitor
    {
        private Expression expression;
        private List<String> locations;

        public LocationFinder(Expression exp)
        {
            this.expression = exp;
        }

        public List<String> Locations
        {
            get
            {
                if (locations == null)
                {
                    locations = new List<string>();
                    this.Visit(this.expression);
                }
                return this.locations;
            }
        }

        /// <summary>
        /// 递归访问二元操作符 Equals 
        /// </summary>
        /// <param name="be"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(Place), "Name"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(Place), "Name"));
                    return be;
                }
                else if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(Place), "State"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(Place), "State"));
                    return be;
                }
                else
                    return base.VisitBinary(be);
            }

            return base.VisitBinary(be);
        }

        /// <summary>
        /// 支持 place.Name.StartsWith("Lond") 这类方法调用
        /// 支持 where places.Contains(place.Name) 这类方法调用
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(String) && m.Method.Name == "StartsWith")
            {
                if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object, typeof(Place), "Name") ||
                    ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object, typeof(Place), "State"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromExpression(m.Arguments[0]));
                    return m;
                }
            }
            else if (m.Method.Name == "Contains")
            {
                Expression valuesExpression = null;

                // 如果是 Enumerable 类型，它是扩展方法，其 Contains 方法有两个参数
                if (m.Method.DeclaringType == typeof(Enumerable))
                {
                    if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[1], typeof(Place), "Name") ||
                    ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[1], typeof(Place), "State"))
                    {
                        // 第一个参数是数据源
                        valuesExpression = m.Arguments[0];
                    }
                }
                // 如果是 List<string>，其 Contains 方法只有一个参数
                else if (m.Method.DeclaringType == typeof(List<string>))
                {
                    if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[0], typeof(Place), "Name") ||
                        ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[0], typeof(Place), "State"))
                    {
                        // 值直接就是数据源
                        valuesExpression = m.Object;
                    }
                }

                if (valuesExpression == null || valuesExpression.NodeType != ExpressionType.Constant)
                    throw new Exception("Could not find the location values.");

                // 转换为常量表达式
                ConstantExpression ce = (ConstantExpression)valuesExpression;

                // 转换为 IEnumerable<string>
                IEnumerable<string> placeStrings = (IEnumerable<string>)ce.Value;
                // Add each string in the collection to the list of locations to obtain data about. 
                foreach (string place in placeStrings)
                {
                    if (!locations.Contains(place))
                    {
                        locations.Add(place);
                    }
                }

                return m;
            }

            return base.VisitMethodCall(m);
        }
    }
}
