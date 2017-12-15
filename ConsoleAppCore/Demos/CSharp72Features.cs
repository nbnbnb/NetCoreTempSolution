using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace ConsoleAppCore.Demos
{
    internal sealed class CSharp72Features
    {
        /// <summary>
        /// C# 7.0 中实现了对数字分隔符的支持，但这不允许文字值的第一个字符是 _
        /// C# 7.2 放开了这一限制
        /// </summary>
        public static void LiteralPrefix()
        {
            Console.WriteLine(0b11_11_11);  // 二进制
            Console.WriteLine(0x12_dd); // 十六进制

            Console.WriteLine(0b_11_11_11);  // 二进制
            Console.WriteLine(0x_12_dd); // 十六进制
        }

        /// <summary>
        /// 新增的 private protected 访问级别
        /// </summary>
        public static void PrivateProtected()
        {
            XYZ xyz = new XYZ();
            //xyz.MethodA(); // private 无法读取
            xyz.MethodB();
        }

        /// <summary>
        /// 非尾随命名参数
        /// </summary>
        /// <param name="delayExecution"></param>
        /// <param name="continueOnError"></param>
        /// <param name="maxRecords"></param>
        private void DoSomething(bool delayExecution, bool continueOnError, int maxRecords)
        {
            // 全部顺序参数
            DoSomething(true, false, 100);
            // 全部命名参数
            DoSomething(delayExecution: true, continueOnError: false, maxRecords: 100);
            // 现在可以在全部命名参数中,指定顺序参数
            // 只要命名参数的位置顺序正确即可
            DoSomething(delayExecution: true, continueOnError: false, 100);
        }

        /// <summary>
        /// C# 7.2 新增 in 关键字来补充现有的 ref 和 out 关键字
        /// in 关键字指定要按引用传递参数，并且调用的方法不会修改传递给它的值
        /// </summary>
        private static double InParam(in Point point1, in Point point2)
        {
            double xDifference = point1.X - point2.X;
            double yDifference = point1.Y - point2.Y;

            // 无法进行赋值
            // point1.X = 123;

            // ref readonly
            // 无法进行赋值
            // Origin.X = 123;

            // 还可以声明一个 ref readonly 变量
            // 右侧需要用 ref 声明
            ref readonly Point originReference = ref origin;

            return Math.Sqrt(xDifference * xDifference + yDifference * yDifference);
        }

        private static Point origin = new Point(1, 2);
        public static ref readonly Point Origin => ref origin;

        private class XYZ
        {
            /// <summary>
            /// 新增的 private protected 
            /// private || protected
            /// </summary>
            private protected void MethodA()
            {

            }

            /// <summary>
            /// internal || protected
            /// </summary>
            internal protected void MethodB()
            {

            }

            public class ABC
            {
                public ABC()
                {
                    XYZ xyz = new XYZ();
                    xyz.MethodA(); // ABC 包含在 XYX 内部，private 也可以读取
                    xyz.MethodB();
                }
            }
        }

        /// <summary>
        /// 使用 readonly struct 声明
        /// 调用 readonly struct 的成员时，编译器将生成更高效的代码：this 引用（in 参数功能）
        /// 这种优化在你使用 readonly struct 时可减少更多复制操作。
        /// 
        /// 存在针对 readonly struct 的其他优化。 
        /// 可在 readonly struct 为参数的每个位置使用 in 修饰符。
        /// 此外，如果要返回其生存期超出返回对象的方法的作用域的对象，可返回 readonly struct 作为 ref return。
        /// </summary>
        public readonly struct ReadonlyPoint3D
        {
            public ReadonlyPoint3D(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public double X { get; }
            public double Y { get; }
            public double Z { get; }

            private static readonly ReadonlyPoint3D origin = new ReadonlyPoint3D();
            // 返回 readonly struct 作为 ref return
            public static ref readonly ReadonlyPoint3D Origin => ref origin;
        }
    }
}
