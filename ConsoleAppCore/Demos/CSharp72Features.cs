using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos
{
    internal sealed class CSharp72Features
    {
        /// <summary>
        /// C# 7.0 中实现了对数字分隔符的支持，但这不允许 0b 或 0x 这类进制标识后的第一个字符是 _
        /// C# 7.2 放开了这一限制
        /// </summary>
        public static void LiteralPrefix()
        {
            // 7.0+
            Console.WriteLine(0b11_11_11);  // 二进制
            Console.WriteLine(0x12_dd); // 十六进制

            // 7.2+
            Console.WriteLine(0b_11_11_11);  // 二进制
            Console.WriteLine(0x_12_dd); // 十六进制
        }

        #region private protected
        /// <summary>
        /// 新增的 private protected 访问级别
        /// 
        /// 系统原先有 internal protected 访问级别
        /// 但是除了继承类可以访问外，当前程序集中的其他成员也可以访问
        /// 程序集外不能访问（不能继承）
        /// 
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
            protected internal void MethodB()
            {

            }
        }

        private class LMN : XYZ
        {
            /// <summary>
            /// 只有继承类才可以访问  private protected
            /// </summary>
            private void CallMethodA()
            {
                base.MethodA();
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
            // 交换位置也支持
            DoSomething(continueOnError: false, delayExecution: true, maxRecords: 100);

            // 现在可以在全部命名参数中,指定顺序参数
            // 只要命名参数的位置顺序正确即可
            DoSomething(delayExecution: true, continueOnError: false, 100);

            // 交换 continueOnError 和 delayExecution 的位置
            // 不支持
            //DoSomething(continueOnError: false, delayExecution: true,  100);
        }

        /// <summary>
        /// 只读引用传递
        /// 在方法调用时
        /// 尤其对值类型有效，避免值类型的赋值
        /// 同时也避免了修改值类型不起效的场景（默认是按值传递的）
        /// </summary>
        private static void ReadOnlyRef(in NormalPoint normalPoint)
        {
            // normalPoint.X 是只读的
            // 同时 normalPoint 是按照引用传递的
            // normalPoint.X = 12;

            // 对于方法调用，将使用防御性副本，因为编译器无法确定方法中是否会修改对象内部的状态
            // 防御性副本是对当前值对象的复制
            // 实际上 normalPoint 没有任何更改
            normalPoint.ChangeX(10);

            // 此处 normalPoint 引用的对象是不会改变的
            // 这样保证了 in 只读的语意
            Console.WriteLine($"OriginX {normalPoint.X}");
        }

        public static void Run()
        {
            NormalPoint k = new NormalPoint(1, 2);
            ReadOnlyRef(k);
            // 不会变
            Console.WriteLine($"ChangeEnd {k.X}");

            ref readonly NormalPoint zero = ref NormalPoint.NNN;
            // 由于是 ref readonly，所有不能修改
            // zero.X = 100;

            // 如果赋值给了一个非 ref readonly 得变量
            // 值对象将会被赋值，同时新变量也失去了限制
            NormalPoint copy = zero;

            copy.X = -100;
            // copy 改变了，原始的 ref NormalPoint 不会变            
            Debug.Assert(copy.X != zero.X);

            // 没有办法通过引用不同的表达式来有条件地绑定变量
            // 类似于三元运算符(也称为条件运算符)在通过值绑定时所做的
            int a = 1;
            int b = 2;
            ref int max = ref b; // a,b 变量需要先初始化s
            if (a > b)
            {
                max = ref a; // C# 7.2 不支持，7.3 支持了
            }

            // 一种替代的方式
            ref int max2 = ref BindConditionally(a > b, ref a, ref b);

            int[] emptyArray = { };
            int[] nonEmptyArray = { 1, 2, 3 };

            // 这个方法有个缺陷
            // 就算 emptyArray.Length = 0 , emptyArray[0] 访问也会被执行
            // 所以会导致 IndexOutOfRangeException 
            // ref int firstItem = ref BindConditionally(emptyArray.Length > 0, ref emptyArray[0], ref nonEmptyArray[0]);

            // 7.2 中新增了 ref conditional expression，解决了这个问题
            // 7.3 中已经支持了 ref rebind，所以也可以通过上面的方式实现这类三目运算符
            ref int firstItem = ref (emptyArray.Length > 0 ? ref emptyArray[0] : ref nonEmptyArray[0]);

        }

        private static ref T BindConditionally<T>(bool condition, ref T trueExpression, ref T falseExpression)
        {
            if (condition)
            {
                return ref trueExpression;
            }
            else
            {
                return ref falseExpression;
            }
        }



        /// <summary>
        /// 引用类型也支持
        /// 不过使用场景作用不是很大
        /// </summary>
        /// <param name="abc"></param>
        private static void ReadOnlyRef(in ABC abc)
        {
            // 不能修改引用地址
            // abc = null;
        }

        /// <summary>
        /// 另一个新特效
        /// readonly struct
        /// 编译器会检测，没有改变内部数据的方法（属性要为只读版本）
        /// 
        /// 当把 readonly struct 结构当做 in 方式进行传递时
        /// 编译器不需要为调用这些结构体的方法而创建防御性副本，因为它知道它们不能修改结构体。
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

            /// <summary>
            /// 使用 in (refernece) 方法传递的对象
            /// 调用这个方法时
            /// 不用创建防御性副本，因为它知道它们不能修改结构体
            /// </summary>
            public void Say()
            {

            }
        }

        /// <summary>
        /// 声明一个 ref struct
        /// 必须在堆栈上分配的 struct
        /// 限制：
        ///     1，不能装箱，例如转换为 Object、dynamic 类型
        ///     2，不能将其声明为其他类型或结构的成员字段
        ///     3，不能在异步方法中声明 ref struct 变量（解决方案，在同步方法中声明，然后返回 Task）
        ///     4，不能在迭代器中声明 ref struct 变量
        ///     5，不能在 lambda 表达式和本地函数中 "捕获" ref struct 变量
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

            public void ChangeX(int plus)
            {
                Console.WriteLine($"ChangeX Begin {X}");
                X = X + plus;
                Console.WriteLine($"ChangeX End {X}");

                Console.WriteLine(this);
            }

            // 创建一个 Read-only return values by reference
            // 类似于一个 in reference
            private static readonly NormalPoint nnn = new NormalPoint(100, 200);
            public static ref readonly NormalPoint NNN => ref nnn;

            public override string ToString()
            {
                return $"X:{X} Y:{Y}";
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
                RefPoint refPoint = new RefPoint(1, 2);
                // 无法装箱成 Object
                // Object aa = a;
                // 无法赋值给 dynamic
                // dynamic aaa = a;

                NormalPoint normalPoint = new NormalPoint(1, 2);
                object bb = normalPoint;
                dynamic bbb = normalPoint;

                ReadOnlyPoint c = new ReadOnlyPoint(1, 2);
                // ReadOnlyPoint 不允许修改
                //c.X = 111;


                Action action = () =>
                {
                    // lambda 无法捕获 refPoint
                    // Console.WriteLine(refPoint.ToString());
                    Console.WriteLine(normalPoint.ToString());
                };

                void myLocalMethod()
                {
                    // 本地函数中 无法捕获 refPoint
                    // Console.WriteLine(refPoint.ToString());
                    Console.WriteLine(normalPoint.ToString());
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
