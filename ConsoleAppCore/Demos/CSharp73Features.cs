namespace ConsoleAppCore.Demos
{
    using ConsoleAppCore.Demos.Misc;
    using System;

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
        /// 增强的泛型约束
        /// 支持 System.Enum 和 System.Delegate
        /// </summary>
        static void EnhanceConstraintEnum<T>() where T : Enum
        {

        }

        /// <summary>
        /// 增强的泛型约束
        /// 支持 System.Enum 和 System.Delegate
        /// </summary>
        static void EnhanceConstraintDelegate<T>() where T : Delegate
        {

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
    }

    namespace Misc
    {
        class MyClass { }

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
            /// 扩展初始值设定项中的表达式变量
            /// out 变量声明
            /// 支持 字段初始化，属性初始化，构造函数初始化，查询子句
            /// 
            /// 此处示例为构造函数初始化
            /// </summary>
            /// <param name="i"></param>
            public D(int i) : base(i, out var j)
            {
                Console.WriteLine($"The value of 'j' is {j}");
            }
        }
    }

}
