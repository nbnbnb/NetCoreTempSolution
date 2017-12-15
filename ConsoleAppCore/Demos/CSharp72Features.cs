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
        private static void InParam(in MyPoint myPoint, in ReadOnlyMyPoint readOnlyMyPoint)
        {
            // 无法进行赋值
            // myPoint.X = 123;
            //readOnlyMyPoint.X = 123;
        }

        private static ReadOnlyMyPoint readOnlyMyPoint = new ReadOnlyMyPoint();
        private static MyPoint myPoint = new MyPoint();

        public static void Test()
        {
            // 将 readonly struct 作为 in 参数传递到其成员方法
            InParam(myPoint, readOnlyMyPoint);
        }

        /// <summary>
        /// ref struct 声明，指示结构类型直接访问托管的内存（栈）
        /// ref struct 声明，指示结构类型直接访问托管的内存，且必须始终由堆栈分配
        /// 
        /// ref struct 的限制
        ///     1，不能对 ref struct 装箱。 无法向属于 object、dynamic 或任何接口类型的变量分配 ref struct 类型。
        ///     2，不能将 ref struct 声明为类或常规结构的成员。
        ///     3，不能声明异步方法中属于 ref struct 类型的本地变量。 不能在返回 Task、Task<T> 或类似 Task 类型的同步方法中声明它们。
        ///     4，无法在迭代器中声明 ref struct 本地变量。
        ///     5，无法捕获 Lambda 表达式或本地函数中的 ref struct 变量。
        ///   
        /// </summary>
        private static ref MyPoint GetRefMyPoint()
        {
            return ref myPoint;
        }

        private class XYZ
        {
            public int Z { get; set; }

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
        /// 调用 readonly struct 的成员时，编译器将生成更高效的代码：this 引用
        /// 这种优化在你使用 readonly struct 时可减少更多复制操作
        /// 
        /// 存在针对 readonly struct 的其他优化
        ///     可在 readonly struct 为参数的每个位置使用 in 修饰符
        ///     此外，如果要返回其生存期超出返回对象的方法的作用域的对象，可返回 readonly struct 作为 ref return
        /// </summary>
        private readonly struct ReadOnlyMyPoint
        {
            public ReadOnlyMyPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
            // 只读结构中
            // 属性只能设置为只读
            public double X { get; }
            public double Y { get; }
        }

        private struct MyPoint
        {
            public MyPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
