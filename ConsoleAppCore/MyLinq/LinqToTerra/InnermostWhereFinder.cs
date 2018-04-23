using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    /// <summary>
    /// 获取最内层的那个 Where 查询表达式
    /// 所以，不支持多个 Where 表达式的串联查询，只会保留最深层的那一个
    /// </summary>
    class InnermostWhereFinder : ExpressionVisitor
    {
        private MethodCallExpression innermostWhereExpression;

        public MethodCallExpression GetInnermostWhere(Expression expression)
        {
            Visit(expression);
            return innermostWhereExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where")
            {
                innermostWhereExpression = node;
            }

            Visit(node.Arguments[0]);

            return node;
        }
    }
}
