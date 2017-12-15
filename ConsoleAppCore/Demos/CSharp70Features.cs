using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos
{
    public class CSharp70Features
    {
        private class MyPoint
        {
            public MyPoint(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            public double X { get; }
            public double Y { get; }

            /// <summary>
            /// 元组自动解包使用（还可以使用扩展方法）
            /// 名称为 Deconstruct，无返回值
            /// 并且参数都用 out 进行标记
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public void Deconstruct(out double x, out double y)
            {
                x = this.X;
                y = this.Y;
            }
        }

        /// <summary>
        /// 1. Out 变量
        /// </summary>
        public static void OutVar()
        {
            if (Int32.TryParse("111", out Int32 res)) // 还可以使用 out var res
            {
                Console.WriteLine(res);
            }
            else
            {
                Console.WriteLine("Can't parse");
            }

            // res 的作用域在整个方法内部
            Console.WriteLine(res);
        }

        /// <summary>
        /// 2. 模式匹配
        /// 使用 is 进行类型匹配
        /// 相当于之前的 is 和 as 组合使用
        /// 
        /// 有两种模式
        /// is Type value = boolean
        /// switch item case Type value（会将 item 自动转换为匹配类型的 val）
        /// </summary>
        public static int PatternMatching(IEnumerable<Object> values)
        {
            var sum = 0;

            // 使用 is Type value = boolean 模式
            foreach (Object item in values)
            {
                if (item is int val)
                {
                    sum += val;
                }
                else if (item is IEnumerable<Object> subList)
                {
                    sum += PatternMatching(subList);
                }
            }

            // 使用 switch item case Type value 模式
            foreach (Object item in values)
            {
                // 会将 item 自动转换为匹配类型的 val
                switch (item)
                {
                    case int val:
                        sum += val;
                        break;
                    case IEnumerable<Object> subList:
                        sum += PatternMatching(subList);
                        break;
                }
            }

            return sum;
        }

        /// <summary>
        /// 3. 元组功能增强
        /// 简单创建
        /// 变量命名
        /// 自动解包
        /// 返回命名的元组
        /// </summary>
        public static (int Max, int Min) TupleEnhance(IEnumerable<Int32> numbers)
        {
            // 简单的创建方式
            // 使用 Item1,Item2 命名方式
            var letters = ("a", "b");
            Console.WriteLine($"{letters.Item1} - {letters.Item2}");

            // 使用自命名方式（右边）
            var rightAlphabate = (alpha: "a", beta: "b");
            Console.WriteLine($"{rightAlphabate.alpha} - {rightAlphabate.beta}");

            // 使用自命名方式（左边）
            // 如果左右边都指定了，将会保留左边（编译器会提示）
            (string alpha, string beta) leftAlphabate = letters;
            Console.WriteLine($"{leftAlphabate.alpha} - {leftAlphabate.beta}");

            // 解包，并创建“本地”变量
            (String alpha, String beta) = letters;
            Console.WriteLine($"{alpha} - {beta}");  // 变量直接在本地

            int min = int.MinValue;
            int max = int.MaxValue;

            foreach (var n in numbers)
            {
                min = (n < min) ? n : min;
                max = (n > max) ? n : max;
            }

            var point = new MyPoint(1.2, 3.4);

            // 自动解包功能
            // 由于 Point 类定义了相应的 Deconstruct 函数
            // 此处编译器将会自动调用相应的方法
            (double pointX, double pointY) = point;
            Console.WriteLine($"{pointX} - {pointY}");

            // 返回命名的元组
            // 元组通过 (int Max, int Min) 方式定义
            return (max, min);
        }

        /// <summary>
        /// 4. Ref 本地变量和返回值
        /// 
        /// 方法返回值定义为 ref int
        /// </summary>
        /// <returns></returns>
        public static ref int RefLocalAndReturn(int[,] matrix, Func<int, bool> predicate)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (predicate(matrix[i, j]))
                    {
                        // return 返回值定义为 ref
                        return ref matrix[i, j];
                    }
                }
            }

            throw new InvalidOperationException("Not found");
        }

        public static void UseRef()
        {
            int[,] matrix = new int[4, 5];

            matrix[3, 4] = 42;

            // ref 返回值必须用 ref 进行标记
            // ref 返回值只能赋值给 ref 变量（不能赋值给本地变量或属性）
            ref int item = ref RefLocalAndReturn(matrix, val => val == 42);
            Console.WriteLine(matrix[3, 4]);   // 输出 42
            // 此处可以修改 martix 内部的值
            // 通过直接修改的 ref int 变量
            // 所以可以将 ref 变量进行传递，而不用传递 matrix 变量及索引值，从而也可以修改
            item = 555;
            Console.WriteLine(matrix[3, 4]); // 输出 555，修改成功
        }

        /// <summary>
        /// 5. 本地函数
        /// 在返回 Task 之前执行验证
        /// 
        /// 本地函数相对于 lambda 的优势
        /// 对于 lambda 表达式，编译器必须创建一个匿名类，并添加在表达式中使用的变量引用（通过闭包捕获）
        /// lambda 表达式是委托的一个实例，创建委托和 Invoke 执行都会有开销
        /// 本地函数可以在函数内的任意位置声明，而 lambda 只能在调用之前声明
        /// </summary>
        public static Task<String> LocalFunc(string address, int index, string name)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException(message: "An address is required", paramName: nameof(address));
            if (index < 0)
                throw new ArgumentOutOfRangeException(paramName: nameof(index), message: "The index must be non-negative");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(message: "You must supply a name", paramName: nameof(name));

            return longRunningWorkImplementation();

            // 本地函数可以在方法内部任意位置声明
            // 注意本地函数的定义方式（与正常函数类似，不要访问级别修饰符）
            async Task<string> longRunningWorkImplementation()
            {
                await Task.Delay(1000);
                return "haha";
            }
        }

        /// <summary>
        /// 6. 基本异步返回类型
        /// 在一些场景中，频繁的创建 Task 对象，可能会造成一些性能问题，此时需要一个轻量级的 Task 对象
        /// 
        /// ValueTask 可以对现有的 Task 进行包装，实现一些优化处理
        /// 
        /// ValueTask 符合 Awaiter 模式，可以直接在上面执行 await 操作
        /// </summary>
        /// <returns></returns>
        public ValueTask<int> CachedFunc()
        {
            return (cache) ? new ValueTask<int>(cacheResult) : new ValueTask<int>(loadCache());
        }

        private bool cache = false;
        private int cacheResult;
        private async Task<int> loadCache()
        {
            // simulate async work:
            await Task.Delay(100);
            cache = true;
            cacheResult = 100;
            return cacheResult;
        }

        /// <summary>
        /// 7. 在之前 C# 中，throw 一直当作语句使用
        /// 现在，可以在语句中使用 throw 表达式
        /// </summary>
        public static void ThrowExpress()
        {
            String abc = "";
            Console.WriteLine(abc ?? throw new ArgumentException("Not allow null"));
        }

        /// <summary>
        /// 7. 增强的表达式体成员（C# 6.0 只支持成员函数和只读属性）
        /// 现在支持 类型构造器
        /// </summary>
        /// <param name="label"></param>
        // Expression-bodied constructor
        public CSharp70Features(string label) => Label = label;

        /// <summary>
        /// 7. 增强的表达式体成员（C# 6.0 只支持成员函数和只读属性）
        /// 现在支持 析构函数
        /// </summary>
        // Expression-bodied finalizer
        ~CSharp70Features() => Console.Error.WriteLine("Finalized!");

        private string label;

        /// <summary>
        /// 7. 增强的表达式体成员（C# 6.0 只支持成员函数和只读属性）
        /// 现在支持 get/set 访问器
        /// </summary>
        // Expression-bodied get / set accessors.
        public string Label
        {
            get => label;
            set => this.label = value ?? "Default label";
        }

        /// <summary>
        /// 8. 数值字面量增强语法
        /// </summary>
        public static void NumericLiteral()
        {
            // 二进制用前导 0b
            int a = 0b1101;
            Console.WriteLine(a);  // 13

            // 用 _ 分割，提高可读性
            Console.WriteLine(0b11_11_11);  // 等价于 0b111111
            Console.WriteLine(12_34.1234_56_78); // 等价于 1234.12345678
            Console.WriteLine(100_000_000); // 等价于 100000000      

        }
    }



}
