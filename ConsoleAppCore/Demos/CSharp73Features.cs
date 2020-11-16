namespace ConsoleAppCore.Demos
{
    using ConsoleAppCore.Demos.Misc;
    using System;
    using System.ComponentModel;

    internal sealed class CSharp73Features
    {
        static MyStruct s = new MyStruct();

        /// <summary>
        /// 新方式
        /// fixed 字段不需要进行固定
        /// </summary>
        public unsafe static void M()
        {
            int p = s.MyFixedField[5];
            Console.WriteLine(p);
        }

        /// <summary>
        /// 原始方式
        /// 需要声明 int* 变量，并使用 fixed 关键字
        /// </summary>
        public unsafe static void M2()
        {
            fixed (int* ptr = s.MyFixedField)
            {
                int p = ptr[5];
            }
            Console.WriteLine(0);
        }

        /// <summary>
        /// 现在，在对 ref 局部变量进行初始化后
        /// 可以对其重新分配，以引用不同的实例
        /// </summary>
        static void RefAgain(ref VeryLargeStruct veryLargeStruct, ref VeryLargeStruct anotherVeryLargeStruct)
        {
            // 定义一个变量，引用 veryLargeStruct
            ref VeryLargeStruct refLocal = ref veryLargeStruct;
            // 重新引用 anotherVeryLargeStruct
            refLocal = ref anotherVeryLargeStruct;
        }

        /// <summary>
        /// stackalloc 方式的数组元素初始化
        /// 
        /// 注意 Span<int> 形式
        /// </summary>
        static unsafe void InitArray()
        {
            int* pArr = stackalloc int[3] { 1, 2, 3 };
            int* pArr2 = stackalloc int[] { 1, 2, 3 };
            Span<int> arr3 = stackalloc[] { 1, 2, 3 };
        }

        /// <summary>
        /// fixed 语法支持 GetPinnableReference 方法的协议
        /// public ref int GetPinnableReference(){ }
        /// </summary>
        internal static unsafe void FixedWithGetPinnableReference()
        {
            // fixed 语句块
            // MyPinnable 支持 GetPinnableReference 方法的协议
            fixed (int* ptr = new MyPinnable())
            {
                Console.WriteLine(*(ptr + 1));
            }
        }

        /// <summary>
        /// 元组支持 == 和 !=
        /// </summary>
        internal static void TubleEquals()
        {
            var a = (a: 1, b: 2);
            var b = (b: 2, a: 1);
            var c = (a: 1, b: 2);
            Console.WriteLine(a == b); // false，顺序要一致
            Console.WriteLine(a != b); // true
            Console.WriteLine(a == c); // true
        }

        /// <summary>
        /// 将 Attribute 添加到自动属性的后台字段上
        /// </summary>
        [field: SomeThingAboutField]
        public int SomeProperty { get; set; }

        /// <summary>
        /// 将 Attribute 添加到事件的的后台（委托）字段上
        /// 事件 Attribute 早期版本就支持
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 支持 in 的方法重载
        /// in 代表的是引用的只读版本，与之对应的是 ref 和 out 的引用可写版本
        /// 定义了 in 之后，就不能定义 ref 或 out 的重载
        /// </summary>
        /// <param name="i"></param>
        static void SampleMethod(int i)
        {
            i = 123;  // 可以重新赋值
            Console.WriteLine($"Without In {i}");
        }
        static void SampleMethod(in int i)
        {
            // i = 123; 不允许赋值
            Console.WriteLine($"With In {i}");
        }

        public static void InMethodOverload()
        {
            // 不带 in 的版本
            SampleMethod(5);
            //SampleMethod(5L); // CS1503: no implicit conversion from long to int

            short s = 0;
            // 不带 in 的版本
            // 隐式类型转换为 int
            SampleMethod(s); // OK, temporary int created with the value 0
            // 由于 in 的本质是传递的引用
            // 所以此处是不支持转型的
            // SampleMethod(in s); // CS1503: cannot convert from in short to in int

            int i = 42;
            // 不带 in 的版本
            SampleMethod(i); // passed by readonly reference
            // 带 in 的版本
            // 此处需要显式指定 in
            // 显式指定 in
            SampleMethod(in i); // passed by readonly reference, explicitly using `in`
        }

        string input = "1234";

        object weapon = new MyClass();

        /// <summary>
        /// 表达式变量
        /// 属性初始化中使用
        /// </summary>
        public int Arrows => weapon is MyClass myClass ? myClass.X : 0;

        public int ArrowsMethod()
        {
            // 原始方式
            if (weapon is MyClass)
            {
                MyClass myClass = (MyClass)weapon;
                return myClass.X;
            }

            // 表达式变量
            if (weapon is MyClass myClass2)
            {
                return myClass2.X;
            }


            return 0;
        }

        /// <summary>
        /// 表达式变量
        /// out int value
        /// Out 中使用
        /// </summary>
        private int Number => Int32.TryParse(input, out int value) ? value : 0;

        /// <summary>
        /// 原始方式 TryParse 模式
        /// </summary>
        /// <returns></returns>
        private int NumberWithMethod()
        {
            int value = 0;
            if (Int32.TryParse(input, out value))
            {
                return value;
            }
            return -1;
        }

        public static void Run()
        {
            // 使用扩展方法
            Console.WriteLine(ABC.A.GetDescription());

            MyHelperClass.MustValueType(123);
            MyHelperClass.MustValueType(new MyStruct());
            // 包含 String 类型字段，不行
            // MyHelperClass.MustValueType(new MyStructWithStringField());
            // 类不行
            //MyHelperClass.MustValueType(new MyClass());
            // 字符串不行
            //MyHelperClass.MustValueType("123");

        }
    }

    namespace Misc
    {
        class MyClass
        {
            public int X { get; }
        }

        class SomeThingAboutFieldAttribute : Attribute
        {

        }

        /// <summary>
        /// unsafe
        /// fixed
        /// </summary>
        unsafe struct MyStruct
        {
            public fixed int MyFixedField[10];
        }

        struct MyStructWithStringField
        {
            public int MyIntField;
            public string MyStringField;
        }

        struct VeryLargeStruct
        {

        }

        class MyPinnable
        {
            /// <summary>
            /// 符合 ref T 的签名
            /// </summary>
            /// <returns></returns>
            public ref int GetPinnableReference()
            {
                int[] items = { 1, 2, 3, 4, 5 };
                return ref items[3];
            }
        }


        enum ABC
        {
            [Description("AAA")]
            A,
            [Description("BBB")]
            B,
            [Description("CCC")]
            C
        }

        class B
        {
            public B(int i, out int j)
            {
                // j 在此处进行赋值
                j = i + 1;
            }
        }

        class D : B
        {
            /// <summary>
            /// 成员初始值设定项和查询中的表达式变量声明
            /// 表达式变量
            /// 构造函数中使用
            /// </summary>
            /// <param name="m"></param>
            public D(int i) : base(i, out int m)
            {
                // 此处可以引用父类中的 out m
                // 而不用声明两个变量
                Console.WriteLine($"The value of 'm' is {m}");
            }

            /// <summary>
            /// 原始方式
            /// 声明两个变量
            /// </summary>
            /// <param name="i"></param>
            /// <param name="m"></param>
            public D(int i, int mm) : base(i, out mm)
            {
                Console.WriteLine($"The value of 'mm' is {mm}");
            }
        }

        static class MyHelperClass
        {
            /// <summary>
            /// 委托约束
            /// </summary>
            /// <typeparam name="TDelegate"></typeparam>
            /// <param name="source"></param>
            /// <param name="target"></param>
            /// <returns></returns>
            public static TDelegate Combine<TDelegate>(this TDelegate source, TDelegate target)
               where TDelegate : Delegate
            {
                // 可以安全的调用 Combine 方法了
                return (TDelegate)Delegate.Combine(source, target);
            }

            /// <summary>
            /// 枚举约束
            /// </summary>
            /// <typeparam name="TEnum"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
            {
                var members = typeof(TEnum).GetMember(value.ToString());
                var attributes = members[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                return ((DescriptionAttribute)attributes[0]).Description;
            }

            /// <summary>
            /// 非托管的约束类型检查
            /// 
            /// 值类型约束
            /// T 只能为值类型
            /// 
            /// 并且 T 中不能有为 null 字段
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public unsafe static void MustValueType<T>(in T value) where T : unmanaged
            {
                // 限制了值类型，可以使用 fixed 获取固定地址
                // 此处由于没有确切类型，使用了 void* 
                fixed (void* pp = &value)
                {

                }
            }

            /// <summary>
            /// .NET Core 2.1 中新增的类型
            /// .NET Framework 还不支持，可以使用 System.Memory 进行代替
            /// </summary>
            public static void SpanType()
            {
                var arr = new byte[10];
                // Span<T> 是一个新的值类型，表示任意内存的“相邻区域”
                // 无论相应内存是与托管对象相关联，还是通过互操作由本机代码提供，亦或是位于“堆栈”上
                // 能确保安全访问和高性能特性，就像数组一样
                Span<byte> bytes = arr; // Implicit cast from T[] to Span<T>

                // 随后，可以轻松高效地创建 Span，以利用 Span 的 Slice 方法重载仅表示、指向此数组的子集
                // 随后，可以为生成的 Span 编制索引，以编写和读取原始数组中相关部分的数据
                Span<byte> slicedBytes = bytes.Slice(start: 5, length: 2);
            }
        }
    }

}
