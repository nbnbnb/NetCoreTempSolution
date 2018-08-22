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
        unsafe static public void M()
        {
            int p = s.myFixedField[5];
            Console.WriteLine(p);
        }

        /// <summary>
        /// 原始方式
        /// 需要声明 int* 变量，并使用 fixed 关键字
        /// </summary>
        unsafe static public void M2()
        {
            fixed (int* ptr = s.myFixedField)
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
            ref VeryLargeStruct refLocal = ref veryLargeStruct; // initialization
            refLocal = ref anotherVeryLargeStruct; // reassigned, refLocal refers to different storage.
        }

        /// <summary>
        /// 数组元素初始化值
        /// stackalloc 方式也支持
        /// </summary>
        unsafe static void InitArray()
        {
            var arr = new int[3] { 1, 2, 3 };
            var arr2 = new int[] { 1, 2, 3 };

            int* pArr = stackalloc int[3] { 1, 2, 3 };
            int* pArr2 = stackalloc int[] { 1, 2, 3 };
            Span<int> arr3 = stackalloc[] { 1, 2, 3 };
        }

        /// <summary>
        /// fixed 语法支持 GetPinnableReference 方法的协议
        /// </summary>
        internal static unsafe void FixedWithGetPinnableReference()
        {
            fixed (int* ptr = new MyPinnable())
            {
                // 传递的是第3个索引
                // 此处输出第4个索引
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
        /// 事件 Attribute 早期版本就支持
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        // 支持 in 的方法重载
        // in 代表的是引用的自读版本，与之对应的是 ref 和 out 的引用的可写版本
        // 定义了 in 之后，就不能定义 ref 或 out 的重载

        static void SampleMethod(int i)
        {
            Console.WriteLine($"Without In {i}");
        }
        static void SampleMethod(in int i)
        {
            Console.WriteLine($"With In {i}");
        }

        public static void InMethodOverload()
        {
            // 不带 in 的版本
            SampleMethod(5);
            //SampleMethod(5L); // CS1503: no implicit conversion from long to int

            short s = 0;
            // 不带 in 的版本
            // 隐式类型转换
            SampleMethod(s); // OK, temporary int created with the value 0
            //SampleMethod(in s); // CS1503: cannot convert from in short to in int

            int i = 42;
            // 不带 in 的版本
            SampleMethod(i); // passed by readonly reference
            // 带 in 的版本
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

        /// <summary>
        /// 表达式变量
        /// Out 中使用
        /// </summary>
        private int Number => Int32.TryParse(input, out int value) ? value : 0;

        public static void Run()
        {
            // 使用扩展方法
            Console.WriteLine(ABC.A.GetDescription());

            MyHelperClass.MustValueType(123);
            MyHelperClass.MustValueType(new MyStruct());
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

        unsafe struct MyStruct
        {
            public fixed int myFixedField[10];
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
            /// 表达式变量
            /// 构造函数中使用
            /// </summary>
            /// <param name="i"></param>
            public D(int i) : base(i, out var j)
            {
                // 此处可以引用父类中的 out j
                Console.WriteLine($"The value of 'j' is {j}");
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
                var type = typeof(TEnum);
                var member = type.GetMember(value.ToString());
                var attributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                return ((DescriptionAttribute)attributes[0]).Description;
            }

            /// <summary>
            /// 值类型约束
            /// T 只能为值类型
            /// 并且 T 中不能有为 null 在字段
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public unsafe static void MustValueType<T>(in T value) where T : unmanaged
            {
                // 限制了值类型，可以使用 fixed 获取固定地址
                fixed (void* pp = &value)
                {

                }
            }
        }
    }

}
