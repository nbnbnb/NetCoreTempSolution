using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// 
        /// 静态帮助方法，将变量表达式执行求值操作
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            // 首先使用 Nominator 方法将可以求值的表达式存储到 HashSet 中
            // SubtreeEvaluator 的 Eval 方法通过这个集合，判断表达式需不需要执行求值操作
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// 
        /// 静态帮助方法，将变量表达式执行求值操作
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }

        /// <summary>
        /// 一个委托对象
        /// 不为 ParameterExpression 的表达式都可以被求值
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// 
        /// 根据 HashSet，判断表达式是否需要执行求值操作
        /// </summary>
        class SubtreeEvaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;
            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            /// <summary>
            /// 递归调用
            /// 如果集合存在，则执行求值操作
            /// </summary>
            /// <param name="exp"></param>
            /// <returns></returns>
            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }

                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }

            /// <summary>
            /// 执行计算操作
            /// 将 Expression 转换为 LambdaExpression
            /// 然后执行编译操作
            /// 最后动态调用，转换为 ConstantExpression
            /// </summary>
            /// <param name="e"></param>
            /// <returns></returns>
            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// 
        /// 从底至上的分析，判断那些 Expression 节点是需要求值计算的
        /// 通过 fnCanBeEvaluated 委托作为判断凭据
        /// 
        /// 如果符合条件，则添加到一个 HashSet<Expression> 中
        /// </summary>
        class Nominator : ExpressionVisitor
        {
            Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression> candidates;
            // 表示是否可以被求值
            bool cannotBeEvaluated;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="fnCanBeEvaluated"></param>
            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            /// <summary>
            /// 传递 Expression 进行验证
            /// 递归调用 Visit
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            /// <summary>
            /// 递归调用 Visit
            /// 如果最内层的无法计算，则外层对象也无法计算
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            // 标识当前表达式不能被求值
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }
    }
}
