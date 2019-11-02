using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.CSharp80
{
    #region Readonly 成员（用于结构成员）
    public struct Point
    {
        private double _x;

        /// <summary>
        /// 添加了 readonly 修饰符
        /// 
        /// 编译器会假设 get 访问器可以修改状态
        /// 所以必须显式声明 readonly
        /// </summary>
        public readonly double X
        {
            get { return _x; }
        }

        /// <summary>
        ///  自动实现的属性是一个例外
        ///  编译器将所有自动实现的 Getter 视为 readonly
        ///  因此，此处无需向 Y 属性添加 readonly 修饰符
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// 添加了 readonly 修饰符
        /// </summary>
        public readonly double Distance => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// 一般来说，ToString 方法不会修改状态
        /// 可以将 readonly 修饰符添加到这个方法上
        /// 
        /// 编译器知道 X,Y 属性不会更改状态
        /// 但是 Distance 属性无法判断，所以此次也需要添加 readonly 修饰符
        /// </summary>
        /// <returns></returns>
        public readonly override string ToString() =>
            $"({X}, {Y}) is {Distance} from the origin";

        /*
         * 编译器确实会强制执行 readonly 成员不修改状态的规则
         * 除非删除 readonly 修饰符，否则不会编译以下方法：
        public readonly void Translate(int xOffset, int yOffset)
        {
            X += xOffset;
            Y += yOffset;
        }
        */
    }

    /// <summary>
    /// 之前的结构声明语法
    /// 新功能结构成员声明将会更精细
    /// </summary>
    public readonly struct Point2
    {
        /// <summary>
        /// 只能有 get，不能有 set
        /// </summary>
        public int X { get; }

        public Point2(int x)
        {
            // 只能在初始化的时候赋值
            this.X = x;
        }
    }
    #endregion

    #region 默认接口方法

    public interface IKKKing
    {
        /// <summary>
        /// 声明默认的函数
        /// </summary>
        public void SayHello()
        {
            Console.WriteLine("IKKKing Say Hello");
        }

        /// <summary>
        /// 声明默认的属性
        /// </summary>
        public int Age => 30;
    }

    public class KKKing : IKKKing
    {
        // 使用默认的接口函数和属性
    }

    public class KKKing02 : IKKKing
    {
        /// <summary>
        /// 重写默认的函数
        /// 
        /// 注意：没有 override 修饰符
        /// </summary>
        public void SayHello()
        {
            Console.WriteLine("KKKing02 Say Hello");
        }

        /// <summary>
        /// 重写默认的属性
        /// 
        /// 注意：没有 override 修饰符
        /// </summary>
        public int Age => 33;
    }

    #endregion

    #region 更多的 switch 表达式模式
    public enum Rainbow
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet
    }

    public class RGBColor
    {
        public RGBColor(int r, int g, int b)
        {

        }

        // 新增的方式（表达式）
        public static RGBColor FromRainbow(Rainbow colorBand)
            // 变量 colorBand 位于 switch 关键字之前
            => colorBand switch
            {
                // case 和 : 替换为了 =>
                Rainbow.Red => new RGBColor(0xFF, 0x00, 0x00),
                Rainbow.Orange => new RGBColor(0xFF, 0x7F, 0x00),
                Rainbow.Yellow => new RGBColor(0xFF, 0xFF, 0x00),
                Rainbow.Green => new RGBColor(0x00, 0xFF, 0x00),
                Rainbow.Blue => new RGBColor(0x00, 0x00, 0xFF),
                Rainbow.Indigo => new RGBColor(0x4B, 0x00, 0x82),
                Rainbow.Violet => new RGBColor(0x94, 0x00, 0xD3),
                // default 用 _ 进行了替换
                _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(colorBand))
            };


        // 这是经典的方式（语句）
        // 可以与上面的进行比较
        public static RGBColor FromRainbowClassic(Rainbow colorBand)
        {
            switch (colorBand)
            {
                case Rainbow.Red:
                    return new RGBColor(0xFF, 0x00, 0x00);
                case Rainbow.Orange:
                    return new RGBColor(0xFF, 0x7F, 0x00);
                case Rainbow.Yellow:
                    return new RGBColor(0xFF, 0xFF, 0x00);
                case Rainbow.Green:
                    return new RGBColor(0x00, 0xFF, 0x00);
                case Rainbow.Blue:
                    return new RGBColor(0x00, 0x00, 0xFF);
                case Rainbow.Indigo:
                    return new RGBColor(0x4B, 0x00, 0x82);
                case Rainbow.Violet:
                    return new RGBColor(0x94, 0x00, 0xD3);
                default:
                    throw new ArgumentException(message: "invalid enum value", paramName: nameof(colorBand));
            };
        }

        public class Address
        {
            public String State { get; set; }

            public String Country { get; set; }
        }

        /// <summary>
        /// 属性模式
        /// </summary>
        /// <param name="location"></param>
        /// <param name="salePrice"></param>
        /// <returns></returns>

        public static decimal ComputeSalesTax(Address location, decimal salePrice)
            => location switch
            {
                // 匹配其中一个属性
                { State: "WA" } => salePrice * 0.06M,
                { State: "MN" } => salePrice * 0.75M,
                // 还可以匹配多个属性
                { State: "MI", Country: "USA" } => salePrice * 0.05M,
                // 默认值
                _ => 0M
            };

        /// <summary>
        /// 元组模式
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string RockPaperScissors(string first, string second)
            // 将两个参数组合到一个元组中
            => (first, second) switch
            {
                // 此处通过元组的相等性规则进行匹配
                ("rock", "paper") => "rock is covered by paper. Paper wins.",
                ("rock", "scissors") => "rock breaks scissors. Rock wins.",
                ("paper", "rock") => "paper covers rock. Paper wins.",
                ("paper", "scissors") => "paper is cut by scissors. Scissors wins.",
                ("scissors", "rock") => "scissors is broken by rock. Rock wins.",
                ("scissors", "paper") => "scissors cuts paper. Scissors wins.",
                (_, _) => "tie"
            };

        public class Point3
        {
            public int X { get; }
            public int Y { get; }

            public Point3(int x, int y) => (X, Y) = (x, y);

            /// <summary>
            /// 包含了 Deconstruct 方法
            /// 可以通过元组进行提取
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public void Deconstruct(out int x, out int y) =>
                (x, y) = (X, Y);
        }

        public enum Quadrant
        {
            Unknown,
            Origin,
            One,
            Two,
            Three,
            Four,
            OnBorder
        }

        /// <summary>
        /// 位置模式
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        static Quadrant GetQuadrant(Point3 point)
            => point switch
            {
                (0, 0) => Quadrant.Origin,
                // 元组是自动解包生产的
                // 使用 var 声明，when 比较的方式
                var (x, y) when x > 0 && y > 0 => Quadrant.One,
                var (x, y) when x < 0 && y > 0 => Quadrant.Two,
                var (x, y) when x < 0 && y < 0 => Quadrant.Three,
                var (x, y) when x > 0 && y < 0 => Quadrant.Four,
                var (_, _) => Quadrant.OnBorder,
                _ => Quadrant.Unknown
            };
    }


    #endregion

    #region using 声明

    public class NewUsing
    {
        static int WriteLinesToFile(IEnumerable<string> lines)
        {
            // 声明一个 using 对象
            // 注意，没有用 { }
            using var file = new System.IO.StreamWriter("WriteLines2.txt");
            // Notice how we declare skippedLines after the using statement.
            int skippedLines = 0;
            foreach (string line in lines)
            {
                if (!line.Contains("Second"))
                {
                    file.WriteLine(line);
                }
                else
                {
                    skippedLines++;
                }
            }
            // Notice how skippedLines is in scope here.
            return skippedLines;
            // 封闭范围的末尾进行处理（回收）
            // file is disposed here
        }

        static int WriteLinesToFileClassic(IEnumerable<string> lines)
        {
            // We must declare the variable outside of the using block
            // so that it is in scope to be returned.
            int skippedLines = 0;
            using (var file = new System.IO.StreamWriter("WriteLines2.txt"))
            {
                foreach (string line in lines)
                {
                    if (!line.Contains("Second"))
                    {
                        file.WriteLine(line);
                    }
                    else
                    {
                        skippedLines++;
                    }
                }
            } // file is disposed here
            return skippedLines;
        }
    }

    #endregion

    #region 静态本地函数
    public class EONFEI
    {
        int M1()
        {
            int y;
            LocalFunction();
            return y;

            // 此处不能声明为 static
            // 因为它包含了对本地变量 y 的引用
            void LocalFunction() => y = 0;
        }

        int M2()
        {
            int y = 5;
            int x = 7;
            return Add(x, y);

            // 这个方法可以向本地函数添加 static 修饰符
            // 已确保本地函数不会从封闭访问捕获（引用）任何变量
            static int Add(int left, int right) => left + right;
        }
    }
    #endregion

    #region 异步流

    public class ERUEOIUR
    {
        /// <summary>
        /// 这个方法返回了一个异步流
        /// 
        /// 有三个属性
        /// 1，用 async 修饰
        /// 2，返回 IAsyncEnumerable<T>
        /// 3，方法体中连续返回的 yield return 语句
        /// </summary>
        public static async IAsyncEnumerable<int> GenerateSequence()
        {
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(100);
                yield return i;
            }
        }

        public static async void KKKing()
        {
            // 可以使用 await foreach 语句来枚举序列
            await foreach (var number in GenerateSequence())
            {
                Console.WriteLine(number);
            }
        }
    }

    #endregion

    #region 索引和范围

    public class ERYEURY
    {
        /*
         * 新增的两个类型 
         * 
         * System.Index 表示一个序列索引
         * System.Range 表示序列的子范围
         * 
         * ^ 表示末尾运算符
         * ^n == sequence.Length - n
         * 所以
         * sequence[^0] == sequence[sequence.Length]
         * 
         * .. 表示范围运算符（包含开始，不包含介绍）
         * [0..^0] == [0..sequence.Length]，表示整个范围
         */


        public void Temp()
        {
            var words = new string[]
                {
                                // index from start    index from end
                    "The",      // 0                   ^9
                    "quick",    // 1                   ^8
                    "brown",    // 2                   ^7
                    "fox",      // 3                   ^6
                    "jumped",   // 4                   ^5
                    "over",     // 5                   ^4
                    "the",      // 6                   ^3
                    "lazy",     // 7                   ^2
                    "dog"       // 8                   ^1

                };              // 9 (or words.Length) ^0

            // ^1 表示最后一个词 dog（sequence.Length - 1 = 8，表示第 8 个索引）
            Console.WriteLine($"The last word is {words[^1]}");

            // 包含的是索引 1,2,3
            // 不包括结尾的 4
            string[] quickBrownFox = words[1..4];

            // 解析出来是 [7..9]
            // 索引，包含的索引是 7,8
            // 不包含 9
            string[] lazyDog = words[^2..^0];

            // 这个可以表示整个数组
            // 可以用这个方式进行数组元素的复制
            string[] allWords = words[..];

            // 表示 [0..4]
            // 0,1,2,3 索引内容
            string[] firstPhrase = words[..4];

            // 表示 [6..9]
            // 6,7,8 索引内容
            string[] lastPhrase = words[6..];

            // 声明 System.Range 类型
            System.Range phrase = 1..4;
            // 然后再 [ 和 ] 中使用它
            string[] text = words[phrase];

            // 不仅数组支持索引和范围
            // String，Span<T> 或 ReadOnlySpan<T> 也支持这种方式
            // https://docs.microsoft.com/zh-cn/dotnet/csharp/tutorials/ranges-indexes
        }
    }

    #endregion

    #region Null 合并赋值

    public class EOFEIF
    {
        public void KKKing()
        {
            List<int> numbers = null;
            int? i = null;

            // 仅当左侧 numbers 为 null 时
            // 才将右侧的值分配给左侧
            numbers ??= new List<int>();  // 右侧赋值初始化
            numbers.Add(i ??= 17);  // i 变成 17 了
            numbers.Add(i ??= 20);  // 由于 i 已经有值了，此处不会赋值为 20

            // 注意，与 i ?? 17 的区别，这个时不会赋值的，i 总是为 null

            Console.WriteLine(string.Join(" ", numbers));  // output: 17 17
            Console.WriteLine(i);  // output: 17
        }
    }

    #endregion

    #region 非托管构造类型

    /*
     * 在 C# 7.3 及更低版本中，构造类型（包含至少一个类型参数的类型）不能为非托管类型
     * 例如下面的 Coords<T> 就是一个构造类型，包含一个类型参数 T
     * 
     * 从 C# 8.0 开始，如果构造的值类型仅包含非托管类型的字段，则该类型不受管理。
     * 
     * 从 C# 7.3 开始，可使用 unmanaged 约束指定：类型参数为“非指针非托管类型”
     * 例如  public struct Coords<T> where T : unmanaged
     */

    public struct Coords<T>
    {
        public T X;
        public T Y;
    }

    public class FKEFJKJG
    {
        public void KKKing()
        {
            // Coords<int> 是一个构造类型
            // 但是 int 是一个值类型的非托管类型字段
            // 所以，此处不受约束
            // 与任何非托管类型一样，可以创建指向此类型的变量的指针，或针对此类型的实例在堆栈上分配内存块
            Span<Coords<int>> coordinates = stackalloc[]
            {
                new Coords<int> { X = 0, Y = 0 },
                new Coords<int> { X = 0, Y = 3 },
                new Coords<int> { X = 4, Y = 0 }
            };

            DisplaySize<Coords<int>>();
            // String 不满足 T : unmanaged 约束条件
            // DisplaySize<Coords<String>>();
        }

        /// <summary>
        /// 使用 unmanaged 进行约束
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private unsafe static void DisplaySize<T>() where T : unmanaged
        {
            Console.WriteLine($"{typeof(T)} is unmanaged and its size is {sizeof(T)} bytes");
        }
    }

    #endregion

    #region 嵌套表达式中的 stackalloc
    public class BNKDKJ
    {
        public void KKKing()
        {
            // 从 C# 8.0 开始，如果 stackalloc 表达式的结果为 System.Span<T> 或 System.ReadOnlySpan<T> 类型
            // 则可以在其他表达式中使用 stackalloc 表达式

            Span<int> numbers = stackalloc[] { 1, 2, 3, 4, 5, 6 };

            // 在 IndexOfAny 参数表达式中使用 stackalloc 表达式
            var ind = numbers.IndexOfAny(stackalloc[] { 2, 4, 6, 8 });
            Console.WriteLine(ind);  // output: 1
        }
    }
    #endregion

    #region 内插逐字字符串的增强功能

    public class BNKJGKEG
    {
        public void KKKing()
        {
            // 内插逐字字符串中 $ 和 @ 标记的顺序可以任意安排：$@"..." 和 @$"..." 均为有效的内插逐字字符串
            // 在早期 C# 版本中，$ 标记必须出现在 @ 标记之前

            int a = 124;
            double b = 556.34;

            Console.WriteLine(@$"a={a} \t b={b}");
            Console.WriteLine($@"a={a} \t b={b}");

            // 这个没有用 @ 修饰
            // 将会输出一个 Tab
            Console.WriteLine($"a={a} \t b={b}");
        }
    }

    #endregion

}
