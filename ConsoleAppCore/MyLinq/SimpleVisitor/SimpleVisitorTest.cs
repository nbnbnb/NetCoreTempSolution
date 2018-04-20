using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.SimpleVisitor
{
    class SimpleVisitorTest
    {
        public static void Run()
        {
            DoAggregate();
        }

        static void BuildTree()
        {
            var one = Expression.Constant(1, typeof(int));
            var two = Expression.Constant(2, typeof(int));
            var oneAddTwo = Expression.Add(one, two);
            var lambda = Expression.Lambda<Func<int>>(oneAddTwo);

            Console.WriteLine(lambda.Compile().Invoke());

            // 原型
            //Expression<Func<double, double, double>> distanceCalc =
            //    (x, y) => Math.Sqrt(x * x + y * y);

            var xParameter = Expression.Parameter(typeof(double), "x");
            var yParameter = Expression.Parameter(typeof(double), "y");

            var xSquared = Expression.Multiply(xParameter, xParameter);
            var ySquared = Expression.Multiply(yParameter, yParameter);

            var sum = Expression.Add(xSquared, ySquared);

            var sqrtMethod = typeof(Math).GetMethod("Sqrt", new[] { typeof(double) });
            var distance = Expression.Call(sqrtMethod, sum);

            var distanceLambda = Expression.Lambda<Func<double, double, double>>(distance, xParameter, yParameter);

            Console.WriteLine(distanceLambda.Compile().Invoke(2, 3));

            // 原型
            /*
            Func<int, int> factorialFunc = (n) =>
            {
                var res = 1;
                while (n > 1)
                {
                    res = res * n;
                    n--;
                }
                return res;
            };
            */

            var nArgument = Expression.Parameter(typeof(int), "n");
            var result = Expression.Variable(typeof(int), "result");

            // 用于中断循环的标签目标
            LabelTarget label = Expression.Label(typeof(int));

            // 初始化变量赋值
            var initializeResult = Expression.Assign(result, Expression.Constant(1, typeof(int)));

            // while 内部的语句块
            BlockExpression block = Expression.Block(
                Expression.Assign(result, Expression.Multiply(result, nArgument)),
                Expression.PostDecrementAssign(nArgument));

            // while 循环
            BlockExpression body = Expression.Block(
                new[] { result },  // 定义方法体内的变量
                initializeResult,
                Expression.Loop(
                        Expression.IfThenElse(
                            Expression.GreaterThan(nArgument, Expression.Constant(1, typeof(int))), // 条件判断
                            block, // 满足条件执行的 Block
                            Expression.Break(label, result)  // 不满足条件执行的中断，将 result 赋值给标签值
                        ),
                        label  // 传递跳转使用的标签
                    )
                );

            Console.WriteLine(Expression.Lambda<Func<int, int>>(body, nArgument).Compile().Invoke(5));

        }

        static void ShowTree()
        {
            Expression<Func<int, int, int>> addition = (a, b) => a + b;
            Expression<Func<int, int>> sum = (a) => 1 + a + 3 + 4;
            Expression<Func<int, int>> factorial = (n) =>
                n == 0 ?
                1 :
                Enumerable.Range(1, n).Aggregate((product, factor) => product * factor);

            // 输出表达式结构信息
            Visitor.CreateFromExpression(factorial).Visit("");
        }

        static Expression ReplaceNodes(Expression original)
        {
            if (original.NodeType == ExpressionType.Constant)
            {
                return Expression.Multiply(original, Expression.Constant(10));
            }
            else if (original.NodeType == ExpressionType.Add)
            {
                var binaryExprsssion = (BinaryExpression)original;
                return Expression.Add(ReplaceNodes(binaryExprsssion.Left), ReplaceNodes(binaryExprsssion.Right));
            }

            return original;
        }

        static void TransTree()
        {
            var one = Expression.Constant(1, typeof(int));
            var two = Expression.Constant(2, typeof(int));

            var addition = Expression.Add(one, two);
            var sum = ReplaceNodes(addition);

            var executableFun = Expression.Lambda<Func<int>>(sum);

            Console.WriteLine(executableFun.Compile().Invoke());

        }

        static int Aggregate(Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                var constantExp = (ConstantExpression)exp;
                Console.Error.WriteLine($"Found Constant: {constantExp.Value}");
                return (int)constantExp.Value;
            }
            else if (exp.NodeType == ExpressionType.Add)
            {
                var addExp = (BinaryExpression)exp;
                Console.Error.WriteLine("Found Addition Expression");
                Console.Error.WriteLine("Computing Left node");

                var leftOperand = Aggregate(addExp.Left);  // 递归调用 Left

                Console.Error.WriteLine($"Left is: {leftOperand}");
                Console.Error.WriteLine("Computing Right node");

                var rightOperand = Aggregate(addExp.Right); // 递归调用 Right

                Console.Error.WriteLine($"Right is: {rightOperand}");
                var sum = leftOperand + rightOperand;
                Console.Error.WriteLine($"Computed sum: {sum}");
                return sum;
            }
            else throw new NotSupportedException("Haven't written this yet");
        }

        static Func<Expression, int> SelfAggregate()
        {
            // Declare the delegate, so we can call it 
            // from itself recursively:
            Func<Expression, int> aggregate = null;

            // Aggregate, return constants, or the sum of the left and right operand.
            // Major simplification: Assume every binary expression is an addition.
            aggregate = (exp) =>
                exp.NodeType == ExpressionType.Constant ?
                (int)((ConstantExpression)exp).Value :
                aggregate(((BinaryExpression)exp).Left) + aggregate(((BinaryExpression)exp).Right);

            return aggregate;
        }

        static void DoAggregate()
        {

            var one = Expression.Constant(1, typeof(int));
            var two = Expression.Constant(2, typeof(int));
            var three = Expression.Constant(3, typeof(int));
            var four = Expression.Constant(4, typeof(int));

            var oneAddTwo = Expression.Add(one, two);
            var threeAddfour = Expression.Add(three, four);

            var sum = Expression.Add(oneAddTwo, threeAddfour);

            // 递归方式 1
            Console.WriteLine(SelfAggregate()(sum));

            Console.WriteLine();

            // 递归方式 2
            Console.WriteLine(Aggregate(sum));
        }
    }
}
