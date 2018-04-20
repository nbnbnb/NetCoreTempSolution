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
            BuildTree();
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
    }
}
