using ConsoleAppCore.UnitTests.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleAppCore.UnitTests
{
    [TestClass]
    public class SpanDemoUnitTest : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Test01()
        {
            var arr = new byte[10];
            // Span<T> 是一个新的值类型，表示任意内存的“相邻区域”
            // 无论相应内存是与托管对象相关联，还是通过互操作由本机代码提供，亦或是位于“堆栈”上
            // 能确保安全访问和高性能特性，就像数组一样
            Span<byte> bytes = arr; // Implicit cast from T[] to Span<T>

            // 随后，可以轻松高效地创建 Span，以利用 Span 的 Slice 方法重载仅表示、指向此数组的子集
            // 随后，可以为生成的 Span 编制索引，以编写和读取原始数组中相关部分的数据
            Span<byte> slicedBytes = bytes.Slice(start: 5, length: 2);
            slicedBytes[0] = 42;
            slicedBytes[1] = 43;
            Assert.AreEqual(42, slicedBytes[0]);
            Assert.AreEqual(43, slicedBytes[1]);
            Assert.AreEqual(arr[5], slicedBytes[0]);
            Assert.AreEqual(arr[6], slicedBytes[1]);
            bytes[2] = 45; // OK
            Assert.AreEqual(arr[2], bytes[2]);
            Assert.AreEqual(45, arr[2]);
            slicedBytes[2] = 44; // Throws IndexOutOfRangeException
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Test02()
        {
            // Span 不仅仅只能用于访问数组和分离出数组子集
            // 还可用于引用堆栈上的数据
            Span<byte> bytes = stackalloc byte[2]; // Using C# 7.2 stackalloc support for spans
            bytes[0] = 42;
            bytes[1] = 43;
            Assert.AreEqual(42, bytes[0]);
            Assert.AreEqual(43, bytes[1]);
            bytes[2] = 44; // throws IndexOutOfRangeException
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Test03()
        {
            // Span 可用于引用任意指针和长度（如通过本机堆分配的内存）
            IntPtr ptr = Marshal.AllocHGlobal(1);
            try
            {
                Span<byte> bytes;
                unsafe
                {
                    // (void* pointer, int length) 的重载
                    bytes = new Span<byte>((byte*)ptr, 1);
                }
                bytes[0] = 42;
                Assert.AreEqual(42, bytes[0]);
                Assert.AreEqual(Marshal.ReadByte(ptr), bytes[0]);
                bytes[1] = 43; // Throws IndexOutOfRangeException
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        [TestMethod]
        public void Test04()
        {
            // Span<T> 索引器利用 C# 7.0 中引入的 C# 语言功能，即引用返回
            // 索引器使用“引用 T”返回类型进行声明，其中提供为数组编制索引的语义
            // 同时返回对实际存储位置的引用，而不是相应位置上存在的副本
            // public ref T this[int index] { get { ... } }

            // 通过下例，可以最明显地体现这种引用返回类型索引器带来的影响
            // 如将它与不是引用返回类型的 List<T> 索引器进行比较

            // 将结构隐式转换为 Span<T>，则表示他可以在 Point 处进行更新
            Span<MutableStruct> spanOfStructs = new MutableStruct[1];
            // 执行更新
            spanOfStructs[0].Value = 42;
            Assert.AreEqual(42, spanOfStructs[0].Value);
            var listOfStructs = new List<MutableStruct> { new MutableStruct() };
            // 因为每次读取 listOfStructs[0] 时，都将会是一个值类型的拷贝
            // 所以此处的复制总是对一个未命名的对象进行的，编译器阻止了这种行为
            // listOfStructs[0].Value = 42; // Error CS1612: the return value is not a variable

            // 对值类型的变量赋值是可以的
            var kkking = listOfStructs[0];
            kkking.Value = 42;
            var jjj = kkking;
            Assert.AreEqual(kkking.Value, 42);
            Assert.AreEqual(jjj.Value, 42);
            // 直接读取值类型，总是返回一个副本
            Assert.AreEqual(listOfStructs[0].Value, 0);
        }

        [TestMethod]
        public void Test05()
        {
            // Span<T> 的第二个变体为 System.ReadOnlySpan<T>，可启用只读访问
            // 此类型与 Span<T> 基本类似，不同之处在于前者的索引器利用新 C# 7.2 功能来返回“引用只读 T”，而不是“引用 T”
            // 这样就可以处理 System.String 等不可变数据类型。
            // 使用 ReadOnlySpan<T>，可以非常高效地分离字符串，而无需执行分配或复制操作
            string str = "hello, world";
            string worldString = str.Substring(startIndex: 7, length: 5); // 分配新字符串内存
            ReadOnlySpan<char> worldSpan = str.AsSpan().Slice(start: 7, length: 5); // 不分配，只是保存原始字符串中的一个地址引用，构建成 ReadOnlySpan<T>
            Assert.AreEqual('w', worldSpan[0]);
            // 由于是 ReadOnlySpan<T>，此处不能赋值
            // worldSpan[0] = 'a'; // Error CS0200: indexer cannot be assigned to


        }

        [TestMethod]
        public void Test06()
        {
            // Span 的优势还有许多，远不止已提到的这些
            // 例如，Span 支持 reinterpret_cast 的理念，即可以将 Span<byte> 强制转换为 Span<int>（其中，Span<int> 中的索引 0 映射到 Span<byte> 的前四个字节）
            // 这样一来，如果读取字节缓冲区，可以安全高效地将它传递到对分组字节（视作整数）执行操作的方法

            // Spen<T> 是包含引用和长度的值类型
            // private readonly ref T _pointer;
            // private readonly int _length;

            //“引用 T”字段这一概念初看起来有些奇怪，因为其实无法在 C# 或甚至 MSIL 中声明“引用 T”字段
            // 不过，Span<T> 实际上旨在于运行时使用特殊内部类型，可看作是内部实时 (JIT) 类型，由 JIT 为其生成等效的“引用 T”字段
            // 以可能更为熟悉的引用用法为例：

            // 此代码通过引用传递数组中的槽，这样（除优化外）还可以在堆栈上生成引用 T
            // Span<T> 中的“引用 T”有异曲同工之妙，直接封装在结构中
            // 直接或间接包含此类引用的类型被称为“类似引用”的类型
            // C# 7.2 编译器支持在签名中使用引用结构，从而声明这种“类似引用”的类型
            var values = new int[] { 42, 84, 126 };
            AddOne(ref values[2]);
            Assert.AreEqual(127, values[2]);

            // Span<T> 的定义方式可确保操作效率与数组一样高：为 Span 编制索引无需通过计算来确定指针开头及其起始偏移，因为“引用”字段本身已对两者进行了封装
            // 相比之下，ArraySegment<T> 有单独的偏移字段，这就增加了索引编制和数据传递操作的成本
        }

        [TestMethod]
        public void Test07()
        {
            // 与 .NET 库集成

            string input = "123,456";
            int commaPos = input.IndexOf(',');
            int first = int.Parse(input.Substring(0, commaPos));  // 分配字符串
            int second = int.Parse(input.Substring(commaPos + 1));

            ReadOnlySpan<char> inputSpan = input.AsSpan();
            Assert.AreEqual(first, int.Parse(inputSpan.Slice(0, commaPos)));  // 新的 Parse 方法，使用 Span<T> 重载
            Assert.AreEqual(second, int.Parse(inputSpan.Slice(commaPos + 1)));

            // 新的  Parse 重载
            // 类似分析和格式化方法可用于原始类型（如 Int32），其中包括 DateTime、TimeSpan 和 Guid 等核心类型，甚至还包括 BigInteger 和 IPAddress 等更高级别类型。
            // 从 System.Random 到 System.Text.StringBuilder，再到 System.Net.Socket，这些重载的添加有利于轻松高效地处理 {ReadOnly}Span<T> 和 {ReadOnly}Memory<T>


            // 例如 Stream 的新方法， public virtual ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
            // 在以下情况下，ValueTask<T> 是有助于避免执行分配操作的结构：经常要求使用异步方法来同步返回内容
            // 以及不太可能为所有常见返回值缓存已完成任务
            // 运行时可以为结果 true 和 false 缓存已完成的 Task<bool>，但无法为 Task<int> 的所有可能结果值缓存四十亿任务对象

            // 由于相当常见的是 Stream 实现的缓冲方式让 ReadAsync 调用同步完成，因此这一新 ReadAsync 重载返回 ValueTask<int>
            // 也就是说，同步完成的异步 Stream 读取操作可以完全避免执行分配操作
            // ValueTask<T> 也用于其他新重载，如 Socket.ReceiveAsync、Socket.SendAsync、WebSocket.ReceiveAsync 和 TextReader.ReadAsync 重载


            // 此外，在一些情况下，Span<T> 还支持向框架添加在过去引发内存安全问题的方法
            // 假设要创建的字符串包含随机生成的值（如某类 ID）
            // 现在，可能会编写要求分配字符数组的代码，如下所示：
            int length = 100;
            Random rand = new Random(1000);
            var chars = new char[length];  // 在堆上分配
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(rand.Next(0, 10) + '0');
            }
            string id = new string(chars);


            // 可以改用堆栈分配，甚至能够利用 Span<char>，这样就无需使用不安全代码
            // 此方法还利用接受 ReadOnlySpan<char> 的新字符串构造函数，如下所示：

            // 同样，只有在所需空间大小对于堆栈而言足够小时，此方法才有效
            // 如果长度较短（如 32 个字节），可以使用此方法
            // 但如果长度为数千字节，很容易就会引发堆栈溢出问题
            Span<char> chars2 = stackalloc char[length]; // 直接在堆栈上分配
            for (int i = 0; i < chars2.Length; i++)
            {
                chars2[i] = (char)(rand.Next(0, 10) + '0');
            }
            string id2 = new string(chars2); // 还是需要将堆栈上生成的数据复制到字符串中

            // 使用 String.Create 方法，直接写入到字符串的内存
            // 实现此方法是为了分配字符串，并分发可写 Span
            // 执行写入操作后可以在构造字符串的同时填写字符串的内容
            // 在此示例中，Span<T> 的仅限堆栈这一本质非常有用，因为可以保证在字符串的构造函数完成前 Span（引用字符串的内部存储）就不存在
            // chars3 将会根据 length 的长度进行分配
            // 这样便无法在构造完成后使用 Span 改变字符串了
            string id3 = string.Create(length, rand, (Span<char> chars3, Random r) =>
            {
                for (int i = 0; i < chars3.Length; i++)
                {
                    chars3[i] = (char)(r.Next(0, 10) + '0');
                }
            });
            Console.WriteLine(id3);
            Assert.AreEqual(id3.Length, length);
        }

        [TestMethod]
        public void Test08()
        {
            // 除了核心框架类型有新成员外，我们还正在积极开发许多可与 Span 结合使用的新.NET 类型，从而在特定方案中实现高效处理
            // 例如，对于要编写高性能微服务和处理大量文本的网站的开发人员，如果在使用 UTF-8 时无需编码和解码字符串，则性能会大大提升
            // 为此，我们即将添加 System.Buffers.Text.Base64、System.Buffers.Text.Utf8Parser 和 System.Buffers.Text.Utf8Formatter 等新类型
            // 这些类型对字节 Span 执行操作，不仅避免了 Unicode 编码和解码，还能够处理在各种网络堆栈的最低级别中常见的本机缓冲

            /// Formats supported:
            ///     D (default)     nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
            ///     B               {nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn}
            ///     P               (nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn)
            ///     N               nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn

            ReadOnlySpan<byte> utf8Text = Encoding.UTF8.GetBytes("988cd2ba-1fe7-4288-84a6-26c415d172d8|aaaaa");  // 模拟网路字节流
            bool res = Utf8Parser.TryParse(utf8Text, out Guid value, out int bytesConsumed, standardFormat: 'D');
            Assert.IsTrue(res);
            Assert.AreEqual(bytesConsumed, 36);

            // 这并未止步于核心.NET 库一级，而是继续全面影响堆栈。ASP.NET Core 现在严重依赖 Span
            // 例如，在 Span 基础之上编写 Kestrel 服务器的 HTTP 分析程序
            // Span 今后可能会通过较低级别 ASP.NET Core 中的公共 API 公开，如在它的中间件管道中
        }

        private static void AddOne(ref int value) => value += 1;

        private struct MutableStruct { public int Value; }
    }


}
