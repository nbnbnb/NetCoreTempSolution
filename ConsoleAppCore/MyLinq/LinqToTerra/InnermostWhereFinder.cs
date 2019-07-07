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
                // 此处就是在递归的时候，把最内层的 Where 表达式记录下来
                innermostWhereExpression = node;
            }

            // MethodCallExpression 表达式有两个参数，一个是 lambda，一个是返回值
            // 此处继续递归 lambda
            Visit(node.Arguments[0]);

            return node;
        }
    }
}
