using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    class ExpressionTreeHelpers
    {
        /// <summary>
        /// 判断 EqualsExpression
        /// 指定的 MemberExpression 与类型的 MemberName 是否匹配
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="declaringType"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        internal static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName)
        {
            if (exp.NodeType != ExpressionType.Equal)
                return false;

            BinaryExpression be = (BinaryExpression)exp;

            // Assert. 
            // 检测 A=A 这种错误语法
            if (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) &&
                ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName))
                throw new Exception("Cannot have 'member' == 'member' in an expression!");

            return (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) ||
                ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName));
        }

        /// <summary>
        /// 判断指定的 MemberExpression 与类型的 MemberName 是否匹配
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="declaringType"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
        {
            return ((exp is MemberExpression) &&
                (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                (((MemberExpression)exp).Member.Name == memberName));
        }

        /// <summary>
        /// 从 Equals 中获取常量值 
        /// </summary>
        /// <param name="be"></param>
        /// <param name="memberDeclaringType"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        internal static string GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType, string memberName)
        {
            if (be.NodeType != ExpressionType.Equal)
                throw new Exception("There is a bug in this program.");

            // 判断右边是常量还是左边是常量
            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Left;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                {
                    return GetValueFromExpression(be.Right);
                }
            }
            else if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Right;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                {
                    return GetValueFromExpression(be.Left);
                }
            }

            // We should have returned by now. 
            throw new Exception("There is a bug in this program.");
        }

        /// <summary>
        /// 将常量表达式的值解析出来
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string GetValueFromExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return (string)(((ConstantExpression)expression).Value);
            else
                throw new InvalidQueryException(
                    String.Format("The expression type {0} is not supported to obtain a value.", expression.NodeType));
        }
    }
}
