using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ConsoleAppCore.Demos.Misc
{
    public class CustomFormatter
    {
        public static void Run()
        {
            // ABC 类型实现了 IFormattable 接口
            ABC abc = new ABC();

            // 直接使用的 Console.WriteLine 重载
            // IFormattable.ToString() 接口的 format 参数为 KKKing，formatProvider 参数为 null
            //Console.WriteLine("{0:King}", abc);

            // 使用的 String.Format 重载
            // 这是实际项目中的使用方式
            // IFormattable.ToString() 接口的 format 参数为 KKKing，formatProvider 参数为 null
            //Console.WriteLine(String.Format("{0:King}", abc));

            // 没有识别 NoFormat 这个格式
            // 将会调用 Object.ToString() 的重载
            //Console.WriteLine(String.Format("{0:NoFormat}", abc));

            // 传递自定义的 MyProvider
            //Console.WriteLine(String.Format(new MyProvider(), "{0:King}", abc));


            // 注意，此处使用 "{0:King}"
            // Console.WriteLine(String.Format(new MyProvider(), "{0:King}", abc));

            // 注意，此处还传递一个 40 的空白对齐
            // 这个不是 ICustomFormatter.Format 解析后的结果
            // 空白对齐应该是在后面的 Provider 中实现的
            Console.WriteLine(String.Format(new MyProvider(), "{0,40:King}", abc));
        }

        /// <summary>
        /// 实现了 IFormattable 参考  https://docs.microsoft.com/zh-cn/dotnet/standard/base-types/composite-formatting
        /// </summary>
        private class ABC : IFormattable
        {
            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (formatProvider == null)
                {
                    Console.WriteLine("Null formatProvider");
                }
                else if (formatProvider.Equals(CultureInfo.CurrentCulture))
                {
                    Console.WriteLine("CultureInfo.CurrentCulture formatProvider");
                }
                else if (formatProvider.GetType() == typeof(MyProvider))
                {
                    Console.WriteLine("MyProvider formatProvider");
                }
                if (format.Equals("King"))
                {
                    return "ABCKing";
                }
                else
                {
                    // 调用默认的 Object.ToString()
                    return this.ToString();
                }

                /* Decimal 类的 ToString 逻辑
                 * Decimal price=12.34M;
                 * String s=price.ToString("C",new CultureInfo("zh-CN"));

                  Decimal 类的 ToString 逻辑
                  在内部，其 ToString 方法 【IFormattable 接口的实现】
                  发现 formatProvider 实参如果不为 null，所以，会像下面这样处理
                  NumberFormatInfo nfi=(NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
                  然后根据得到的 NumberFormatInfo 对象，进行自定义显示
                **/

            }

            // 这是重载 Object 的 ToString() 方法
            public override string ToString()
            {
                return "This is override ToString()";
            }
        }

        /// <summary>
        /// 自定义 IFormatProvider ICustomFormatter
        /// 
        /// 一个类型实现了该接口，就认为该类型的实例能提供
        /// 对语言文化敏感的格式信息，与调用线程关联的语言文化应被忽略
        /// 
        /// 可以参考 Decimal.ToString 的实现
        /// </summary>
        private class MyProvider : IFormatProvider, ICustomFormatter
        {
            // 首先调用这个方法 
            // 返回一个 ICustomFormatter 接口
            // 由于 MyProvider 也实现了 ICustomFormatter 接口，所以返回 this 就可以了
            object IFormatProvider.GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                {
                    return this;
                }
                // 如果非自定义 FormatProvider
                // 则使用系统默认的
                // 系统中总共有 3 个类型实现了 IFormatProvider 接口
                // CultureInfo 系统默认
                // NumberFormatInfo 和 DataTimeFormatInfo【这两个只是为了简化编程，如果 formatType 类型与自己一致，就返回 this】
                return CultureInfo.CurrentCulture.GetFormat(formatType);
            }

            // 上面返回的 ICustomFormatter 对象后
            // 就接着调用 ICustomFormatter.Format() 方法
            // formatString：格式字符串
            // arg：原始对象
            // formatProvider：关联的 IFormatProvider
            string ICustomFormatter.Format(string formatString, object arg, IFormatProvider formatProvider)
            {
                String res;

                // 首先判断原始对象有没有实现 IFormattable 接口
                IFormattable formattable = arg as IFormattable;
                if (formattable == null)
                {
                    // 兜底
                    // Object.toString()
                    res = arg.ToString();
                }
                else
                {
                    // 系统里面的基元值类型，都实现了此接口
                    res = formattable.ToString(formatString, formatProvider);
                }

                if (arg.GetType() == typeof(ABC))
                {
                    res = "<ABC>" + res + "</ABC>";
                }

                return res;
            }

        }
    }
}
