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
            // Span<T> ��һ���µ�ֵ���ͣ���ʾ�����ڴ�ġ���������
            // ������Ӧ�ڴ������йܶ��������������ͨ���������ɱ��������ṩ�������λ�ڡ���ջ����
            // ��ȷ����ȫ���ʺ͸��������ԣ���������һ��
            Span<byte> bytes = arr; // Implicit cast from T[] to Span<T>

            // ��󣬿������ɸ�Ч�ش��� Span�������� Span �� Slice �������ؽ���ʾ��ָ���������Ӽ�
            // ��󣬿���Ϊ���ɵ� Span �����������Ա�д�Ͷ�ȡԭʼ��������ز��ֵ�����
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
            // Span ������ֻ�����ڷ�������ͷ���������Ӽ�
            // �����������ö�ջ�ϵ�����
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
            // Span ��������������ָ��ͳ��ȣ���ͨ�������ѷ�����ڴ棩
            IntPtr ptr = Marshal.AllocHGlobal(1);
            try
            {
                Span<byte> bytes;
                unsafe
                {
                    // (void* pointer, int length) ������
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
            // Span<T> ���������� C# 7.0 ������� C# ���Թ��ܣ������÷���
            // ������ʹ�á����� T���������ͽ��������������ṩΪ�����������������
            // ͬʱ���ض�ʵ�ʴ洢λ�õ����ã���������Ӧλ���ϴ��ڵĸ���
            // public ref T this[int index] { get { ... } }

            // ͨ�����������������Ե������������÷�������������������Ӱ��
            // �罫���벻�����÷������͵� List<T> ���������бȽ�

            // ���ṹ��ʽת��Ϊ Span<T>�����ʾ�������� Point �����и���
            Span<MutableStruct> spanOfStructs = new MutableStruct[1];
            // ִ�и���
            spanOfStructs[0].Value = 42;
            Assert.AreEqual(42, spanOfStructs[0].Value);
            var listOfStructs = new List<MutableStruct> { new MutableStruct() };
            // ��Ϊÿ�ζ�ȡ listOfStructs[0] ʱ����������һ��ֵ���͵Ŀ���
            // ���Դ˴��ĸ������Ƕ�һ��δ�����Ķ�����еģ���������ֹ��������Ϊ
            // listOfStructs[0].Value = 42; // Error CS1612: the return value is not a variable

            // ��ֵ���͵ı�����ֵ�ǿ��Ե�
            var kkking = listOfStructs[0];
            kkking.Value = 42;
            var jjj = kkking;
            Assert.AreEqual(kkking.Value, 42);
            Assert.AreEqual(jjj.Value, 42);
            // ֱ�Ӷ�ȡֵ���ͣ����Ƿ���һ������
            Assert.AreEqual(listOfStructs[0].Value, 0);
        }

        [TestMethod]
        public void Test05()
        {
            // Span<T> �ĵڶ�������Ϊ System.ReadOnlySpan<T>��������ֻ������
            // �������� Span<T> �������ƣ���֮ͬ������ǰ�ߵ������������� C# 7.2 ���������ء�����ֻ�� T���������ǡ����� T��
            // �����Ϳ��Դ��� System.String �Ȳ��ɱ��������͡�
            // ʹ�� ReadOnlySpan<T>�����Էǳ���Ч�ط����ַ�����������ִ�з�����Ʋ���
            string str = "hello, world";
            string worldString = str.Substring(startIndex: 7, length: 5); // �������ַ����ڴ�
            ReadOnlySpan<char> worldSpan = str.AsSpan().Slice(start: 7, length: 5); // �����䣬ֻ�Ǳ���ԭʼ�ַ����е�һ����ַ���ã������� ReadOnlySpan<T>
            Assert.AreEqual('w', worldSpan[0]);
            // ������ ReadOnlySpan<T>���˴����ܸ�ֵ
            // worldSpan[0] = 'a'; // Error CS0200: indexer cannot be assigned to


        }

        [TestMethod]
        public void Test06()
        {
            // Span �����ƻ�����࣬Զ��ֹ���ᵽ����Щ
            // ���磬Span ֧�� reinterpret_cast ����������Խ� Span<byte> ǿ��ת��Ϊ Span<int>�����У�Span<int> �е����� 0 ӳ�䵽 Span<byte> ��ǰ�ĸ��ֽڣ�
            // ����һ���������ȡ�ֽڻ����������԰�ȫ��Ч�ؽ������ݵ��Է����ֽڣ�����������ִ�в����ķ���

            // Spen<T> �ǰ������úͳ��ȵ�ֵ����
            // private readonly ref T _pointer;
            // private readonly int _length;

            //������ T���ֶ���һ�������������Щ��֣���Ϊ��ʵ�޷��� C# ������ MSIL ������������ T���ֶ�
            // ������Span<T> ʵ����ּ��������ʱʹ�������ڲ����ͣ��ɿ������ڲ�ʵʱ (JIT) ���ͣ��� JIT Ϊ�����ɵ�Ч�ġ����� T���ֶ�
            // �Կ��ܸ�Ϊ��Ϥ�������÷�Ϊ����

            // �˴���ͨ�����ô��������еĲۣ����������Ż��⣩�������ڶ�ջ���������� T
            // Span<T> �еġ����� T��������ͬ��֮�ֱ�ӷ�װ�ڽṹ��
            // ֱ�ӻ��Ӱ����������õ����ͱ���Ϊ���������á�������
            // C# 7.2 ������֧����ǩ����ʹ�����ýṹ���Ӷ��������֡��������á�������
            var values = new int[] { 42, 84, 126 };
            AddOne(ref values[2]);
            Assert.AreEqual(127, values[2]);

            // Span<T> �Ķ��巽ʽ��ȷ������Ч��������һ���ߣ�Ϊ Span ������������ͨ��������ȷ��ָ�뿪ͷ������ʼƫ�ƣ���Ϊ�����á��ֶα����Ѷ����߽����˷�װ
            // ���֮�£�ArraySegment<T> �е�����ƫ���ֶΣ�����������������ƺ����ݴ��ݲ����ĳɱ�
        }

        [TestMethod]
        public void Test07()
        {
            // �� .NET �⼯��

            string input = "123,456";
            int commaPos = input.IndexOf(',');
            int first = int.Parse(input.Substring(0, commaPos));  // �����ַ���
            int second = int.Parse(input.Substring(commaPos + 1));

            ReadOnlySpan<char> inputSpan = input.AsSpan();
            Assert.AreEqual(first, int.Parse(inputSpan.Slice(0, commaPos)));  // �µ� Parse ������ʹ�� Span<T> ����
            Assert.AreEqual(second, int.Parse(inputSpan.Slice(commaPos + 1)));

            // �µ�  Parse ����
            // ���Ʒ����͸�ʽ������������ԭʼ���ͣ��� Int32�������а��� DateTime��TimeSpan �� Guid �Ⱥ������ͣ����������� BigInteger �� IPAddress �ȸ��߼������͡�
            // �� System.Random �� System.Text.StringBuilder���ٵ� System.Net.Socket����Щ���ص�������������ɸ�Ч�ش��� {ReadOnly}Span<T> �� {ReadOnly}Memory<T>


            // ���� Stream ���·����� public virtual ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
            // ����������£�ValueTask<T> �������ڱ���ִ�з�������Ľṹ������Ҫ��ʹ���첽������ͬ����������
            // �Լ���̫����Ϊ���г�������ֵ�������������
            // ����ʱ����Ϊ��� true �� false ��������ɵ� Task<bool>�����޷�Ϊ Task<int> �����п��ܽ��ֵ������ʮ���������

            // �����൱�������� Stream ʵ�ֵĻ��巽ʽ�� ReadAsync ����ͬ����ɣ������һ�� ReadAsync ���ط��� ValueTask<int>
            // Ҳ����˵��ͬ����ɵ��첽 Stream ��ȡ����������ȫ����ִ�з������
            // ValueTask<T> Ҳ�������������أ��� Socket.ReceiveAsync��Socket.SendAsync��WebSocket.ReceiveAsync �� TextReader.ReadAsync ����


            // ���⣬��һЩ����£�Span<T> ��֧����������ڹ�ȥ�����ڴ氲ȫ����ķ���
            // ����Ҫ�������ַ�������������ɵ�ֵ����ĳ�� ID��
            // ���ڣ����ܻ��дҪ������ַ�����Ĵ��룬������ʾ��
            int length = 100;
            Random rand = new Random(1000);
            var chars = new char[length];  // �ڶ��Ϸ���
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(rand.Next(0, 10) + '0');
            }
            string id = new string(chars);


            // ���Ը��ö�ջ���䣬�����ܹ����� Span<char>������������ʹ�ò���ȫ����
            // �˷��������ý��� ReadOnlySpan<char> �����ַ������캯����������ʾ��

            // ͬ����ֻ��������ռ��С���ڶ�ջ�����㹻Сʱ���˷�������Ч
            // ������Ƚ϶̣��� 32 ���ֽڣ�������ʹ�ô˷���
            // ���������Ϊ��ǧ�ֽڣ������׾ͻ�������ջ�������
            Span<char> chars2 = stackalloc char[length]; // ֱ���ڶ�ջ�Ϸ���
            for (int i = 0; i < chars2.Length; i++)
            {
                chars2[i] = (char)(rand.Next(0, 10) + '0');
            }
            string id2 = new string(chars2); // ������Ҫ����ջ�����ɵ����ݸ��Ƶ��ַ�����

            // ʹ�� String.Create ������ֱ��д�뵽�ַ������ڴ�
            // ʵ�ִ˷�����Ϊ�˷����ַ��������ַ���д Span
            // ִ��д�����������ڹ����ַ�����ͬʱ��д�ַ���������
            // �ڴ�ʾ���У�Span<T> �Ľ��޶�ջ��һ���ʷǳ����ã���Ϊ���Ա�֤���ַ����Ĺ��캯�����ǰ Span�������ַ������ڲ��洢���Ͳ�����
            // chars3 ������� length �ĳ��Ƚ��з���
            // �������޷��ڹ�����ɺ�ʹ�� Span �ı��ַ�����
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
            // ���˺��Ŀ���������³�Ա�⣬���ǻ����ڻ������������� Span ���ʹ�õ���.NET ���ͣ��Ӷ����ض�������ʵ�ָ�Ч����
            // ���磬����Ҫ��д������΢����ʹ�������ı�����վ�Ŀ�����Ա�������ʹ�� UTF-8 ʱ�������ͽ����ַ����������ܻ�������
            // Ϊ�ˣ����Ǽ������ System.Buffers.Text.Base64��System.Buffers.Text.Utf8Parser �� System.Buffers.Text.Utf8Formatter ��������
            // ��Щ���Ͷ��ֽ� Span ִ�в��������������� Unicode ����ͽ��룬���ܹ������ڸ��������ջ����ͼ����г����ı�������

            /// Formats supported:
            ///     D (default)     nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
            ///     B               {nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn}
            ///     P               (nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn)
            ///     N               nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn

            ReadOnlySpan<byte> utf8Text = Encoding.UTF8.GetBytes("988cd2ba-1fe7-4288-84a6-26c415d172d8|aaaaa");  // ģ����·�ֽ���
            bool res = Utf8Parser.TryParse(utf8Text, out Guid value, out int bytesConsumed, standardFormat: 'D');
            Assert.IsTrue(res);
            Assert.AreEqual(bytesConsumed, 36);

            // �Ⲣδֹ���ں���.NET ��һ�������Ǽ���ȫ��Ӱ���ջ��ASP.NET Core ������������ Span
            // ���磬�� Span ����֮�ϱ�д Kestrel �������� HTTP ��������
            // Span �����ܻ�ͨ���ϵͼ��� ASP.NET Core �еĹ��� API ���������������м���ܵ���
        }

        private static void AddOne(ref int value) => value += 1;

        private struct MutableStruct { public int Value; }
    }


}
