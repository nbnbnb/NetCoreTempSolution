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
        /// C# 7.0 中实现了对数字分隔符的支持，但这不允许 0b 或 0x 后的第一个字符是 _
        /// C# 7.2 放开了这一限制
        /// </summary>
        public static void LiteralPrefix()
        {
            Console.WriteLine(0b11_11_11);  // 二进制
            Console.WriteLine(0x12_dd); // 十六进制

            Console.WriteLine(0b_11_11_11);  // 二进制
            Console.WriteLine(0x_12_dd); // 十六进制
        }

        #region private protected
        /// <summary>
        /// 新增的 private protected 访问级别
        /// 
        /// 系统原先有 internal protected 访问级别，但是除了继承类可以访问外，程序集中的其他成员也可以访问
        /// private protected 则限制了只有继承的成员才可以访问
        /// 
        /// 嵌套类是可以访问宿主类 private 成员的
        /// </summary>
        public static void PrivateProtected()
        {
            XYZ xyz = new XYZ();
            //xyz.MethodA(); // 外部类，无法访问 private 
            xyz.MethodB();   // internal 可以访问
        }

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
        }
        #endregion

        /// <summary>
        /// 命名参数可以在非命名参数之前
        /// 只要命名参数保持正确位置即可
        /// </summary>
        private void DoSomething(bool delayExecution, bool continueOnError, int maxRecords)
        {
            // 非命名
            DoSomething(true, false, 100);

            // 命名
            DoSomething(delayExecution: true, continueOnError: false, maxRecords: 100);

            // 全部命名参数
            // 交互位置也支持
            DoSomething(continueOnError: false, delayExecution: true, maxRecords: 100);

            // 现在可以在全部命名参数中,指定顺序参数
            // 只要命名参数的位置顺序正确即可
            DoSomething(delayExecution: true, continueOnError: false, 100);

            // 交换 continueOnError 和 delayExecution 的位置
            // 不支持
            //DoSomething(continueOnError: false, delayExecution: true,  100);
        }

        /// <summary>
        /// 声明一个 readonly struct
        /// 
        /// 使用方式
        ///     1，通过 in 参数进行传递（避免值类型的复制）
        /// </summary>
        private readonly struct ReadOnlyPoint
        {
            public ReadOnlyPoint(double x, double y)
            {
                X = x;
                Y = y;
            }
            public double X { get; }
            // 只读结构，不允许有 set
            //public double X { get; set; }
            public double Y { get; }
            // 只读结构，不允许有 set
            //public double Y { get; set; }
        }

        /// <summary>
        /// 声明一个 ref struct
        /// 
        /// 限制：
        ///     1，不能装箱，例如转换为 Object、dynamic 类型
        ///     2，不能将其声明为其他类型或结构的成员字段
        ///     3，不能在异步方法中声明 ref struct 变量（解决方案，在同步方法中声明，然后返回 Task）
        ///     4，不能在迭代器中声明 ref struct 变量
        ///     5，不能在 lambad 表达式和本地函数中 "捕获" ref struct 变量
        ///     6，不能实现接口
        /// </summary>
        private ref struct RefPoint
        {
            public RefPoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }

            public int Y { get; set; }
        }

        /// <summary>
        /// 声明一个正常的 struct
        /// </summary>
        private struct NormalPoint : IComparable<NormalPoint>
        {
            public NormalPoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }

            public int Y { get; set; }

            public int CompareTo(NormalPoint other)
            {
                return X - Y;
            }
        }

        private class ABC
        {
            // ref struct 不能作为类型成员
            // private RefPoint refPoint = new RefPoint(1, 2);

            private NormalPoint normalPoint = new NormalPoint(1, 2);

            // 声明一个 readonly + (readonly struct)
            // 这样就表示这个对象不能被更改（指针和字段）
            private static readonly ReadOnlyPoint _allReadOnly = new ReadOnlyPoint(1, 2);
            // 然后再将其声明为一个引用（避免调用时复制的开销）
            // 调用成员时，编译器生成的代码都是 this 直接调用，避免复制这个结构
            public static ref readonly ReadOnlyPoint allRefReadOnly => ref _allReadOnly;

            public void MyMethod()
            {
                RefPoint a = new RefPoint(1, 2);
                // 无法装箱成 Object
                // Object aa = a;
                // 无法赋值给 dynamic
                // dynamic aaa = a;

                NormalPoint b = new NormalPoint(1, 2);
                Object bb = b;
                dynamic bbb = b;

                ReadOnlyPoint c = new ReadOnlyPoint(1, 2);
                // ReadOnlyPoint 不允许修改
                //c.X = 111;


                Action action = () =>
                {
                    // lambda 无法捕获 a
                    // Console.WriteLine(a.ToString());
                    Console.WriteLine(b.ToString());
                };

                void myLocalMethod()
                {
                    // 本地函数中 无法捕获 a
                    // Console.WriteLine(a.ToString());
                    Console.WriteLine(b.ToString());
                }
            }

            public async void DoAsync()
            {
                // async 方法中不能声明 ref struct
                // RefPoint a = new RefPoint(1, 2);
                NormalPoint b = new NormalPoint(1, 2);
                await Task.Delay(100);
            }
        }
    }

}
