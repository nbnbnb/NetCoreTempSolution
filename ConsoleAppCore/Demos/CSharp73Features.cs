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
            System.Console.WriteLine(p);
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
    }

    namespace Misc
    {
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
    }

}
