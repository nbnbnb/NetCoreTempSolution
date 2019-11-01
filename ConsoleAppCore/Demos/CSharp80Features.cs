using System;
using System.Collections.Generic;

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
    internal sealed class CSharp80Features
    {

        public void Temp()
        {


        }
    }

}
