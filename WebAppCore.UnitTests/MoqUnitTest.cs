using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebAppCore.UnitTests
{
    [TestClass]
    public class MoqUnitTest
    {

        [TestMethod]
        public void Method_Test()
        {
            var mock = new Mock<IFoo>();
            var fakeFoo = mock.Object;
            //����ָ���Ĳ���������ָ���Ľ��
            mock.Setup(m => m.DoSomething("ping")).Returns(true);

            var outString = "ack";
            // ʹ�� out ���������ڲ���ʼ����
            // TryParse will return true, and the out argument will return "ack", lazy evaluated
            mock.Setup(m => m.TryParse("ping", out outString)).Returns(true);

            var bar = new Bar();
            var bar2 = new Bar();
            // ʹ�� ref ���������ⲿ��ʼ����
            // Only matches if the ref argument to the invocation is the same instance
            mock.Setup(m => m.Submit(ref bar)).Returns(true);
            // Setup �Ĳ�������֤�Ĳ���Ҫ��֤��ͬһ��ʵ��
            Assert.IsTrue(fakeFoo.Submit(ref bar));
            // ����ǲ�ͬ��ʵ�������� false
            Assert.IsFalse(fakeFoo.Submit(ref bar2));

            // ���÷���������ֵ
            // ʹ�� It.IsAny<T> ģ���κβ���
            mock.Setup(m => m.DoSomethingStringy(It.IsAny<String>()))
                .Returns<String>(s => s.ToLower());

            // ��֤�׳��쳣
            mock.Setup(m => m.DoSomething("reset")).Throws<InvalidOperationException>();
            Assert.ThrowsException<InvalidOperationException>(() => fakeFoo.DoSomething("reset"));

            // �ӳٳ�ʼ������ֵ
            var count = 1;
            mock.Setup(m => m.GetCount()).Returns(() => count);

        }

        [TestMethod]
        public void Matching_Arguments_Test()
        {
            var mock = new Mock<IFoo>();
            var fakeFoo = mock.Object;

            // Any value
            mock.Setup(m => m.DoSomething(It.IsAny<String>())).Returns(true);
            Assert.IsTrue(fakeFoo.DoSomething(null));
            Assert.IsTrue(fakeFoo.DoSomething(""));
            Assert.IsTrue(fakeFoo.DoSomething(" "));
            Assert.IsTrue(fakeFoo.DoSomething("abc"));

            // Func<T,bool> ƥ��
            mock.Setup(m => m.Add(It.Is<int>(i => i % 2 == 0))).Returns(true);
            Assert.IsFalse(fakeFoo.Add(1));
            Assert.IsTrue(fakeFoo.Add(2));
            // Range ƥ��
            mock.Setup(m => m.Add(It.IsInRange<int>(0, 10, Range.Inclusive))).Returns(true);
            Assert.IsTrue(fakeFoo.Add(9));
            // Regex ƥ��
            mock.Setup(m => m.DoSomethingStringy(It.IsRegex("[a-d]+", RegexOptions.IgnoreCase))).Returns("abcd");
            Assert.IsTrue(fakeFoo.DoSomethingStringy("abcd") == "abcd");
            Assert.IsTrue(fakeFoo.DoSomethingStringy("abcd") == "abcd");
        }

        [TestMethod]
        public void Properties_Test()
        {
            var mock = new Mock<IFoo>();
            var fakeFoo = mock.Object;

            // �������Ը�ֵ
            mock.Setup(m => m.Bar.Baz.Name).Returns("baz");
            Assert.IsTrue(fakeFoo.Bar.Baz.Name == "baz");

            // ����һ�����Ա����ö�Ӧ��ֵ
            mock.SetupSet(m => m.Name = "foo");
            // ִ�������ĸ�ֵ
            fakeFoo.Name = "foo";
            // �����֤�����ֵ�Ƿ�ɹ�
            mock.VerifySet(m => m.Name = "foo");
            //mock.VerifyAll();

            // ���������ж�Ӧ�� property behavior
            // �������ô���ã������޷����и�ֵ��
            // ����
            fakeFoo.Name = "abc";
            // ��ֵû�гɹ�
            Assert.IsTrue(fakeFoo.Name == null);
            // ʹ�� SetupProperty �򿪸�ֵ����
            // Ĭ��ֵΪ null
            mock.SetupProperty(m => m.Name);
            Assert.AreEqual(fakeFoo.Name, null);
            // ����δ��ֵʱ��Ĭ��ֵ
            mock.SetupProperty(m => m.Name, "foo");
            Assert.AreEqual("foo", fakeFoo.Name);
            // ��������ֵ
            fakeFoo.Name = "bar";
            Assert.AreEqual("bar", fakeFoo.Name);

            // �����е��������� property behavior
            mock.SetupAllProperties();
        }

        [TestMethod]
        public void Events_Test()
        {

        }

        [TestMethod]
        public void Callbacks_Test()
        {
            var mock = new Mock<IFoo>();
            var fakeFoo = mock.Object;
            var calls = 0;
            var callArgs = new List<String>();

            mock.Setup(m => m.DoSomething("ping"))
                // �ڷ���ִ�е�ʱ��ִ������ص�
                .Callback(() => calls++)
                .Returns(true);

            mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
                // Callback �н��շ�������
                // �Ƿ���
                .Callback((string s) => callArgs.Add(s))
                .Returns(true);

            // Callback �н��շ�������
            // ����
            mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
                .Callback<string>(s => callArgs.Add(s))
             .Returns(true);

            // �������
            mock.Setup(foo => foo.DoSomething(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((i, s) => callArgs.Add(s))
                .Returns(true);

            // ǰ�����
            mock.Setup(foo => foo.DoSomething("ping"))
                .Callback(() => Console.WriteLine("Before returns"))
                .Returns(true)
                .Callback(() => Console.WriteLine("After returns"));

            // ʹ�� ref/out ����ʱ
            // ��Ҫʹ��һ��ί�в����������
            mock.Setup(foo => foo.Submit(ref It.Ref<Bar>.IsAny))
                .Callback(new SubmitCallback((ref Bar bar) => Console.WriteLine("Submitting a Bar!")));

            // С����
            // ÿ�ε��÷��ز�ͬ��ֵ
            mock.Setup(foo => foo.GetCount())
            .Callback(() => calls++)
            .Returns(() => calls);

        }

        [TestMethod]
        public void Behavior_Test()
        {
            // Ĭ���� Loose ģʽ
            // ������δ Setup ������ʱ��������Ĭ��ֵ
            // ������δ Setup �ķ���ʱ��������Ĭ��ֵ�� void
            // ���Ϸ���ΪԪ��Ϊ 0 �ļ���
            var mock = new Mock<IFoo>(MockBehavior.Default);
            var name = mock.Object.Name;
            Assert.IsFalse(mock.Object.DoSomething("abc"));
            // ���� Strict ģʽ�󣬽��ᱨ��
            mock = new Mock<IFoo>(MockBehavior.Strict);
            Assert.ThrowsException<MockException>(() => name = mock.Object.Name);
            Assert.ThrowsException<MockException>(() => mock.Object.DoSomething("abc"));

            // Mock ���Ƿ�Ҫ���û�������ⷽ��
            // Ĭ��Ϊ False
            // ����ģ�� System.Web �е� Web/Html ����ؼ�����Ҫ��д��Щ����
            var b = new Mock<B> { CallBase = true };
            // ������ø��� A ��� SayName ����
            b.Object.SayName();
            Console.WriteLine("---");
            // ��� Setup �˷���
            // �򲻻���ø���ķ���
            b.Setup(m => m.SayName()).Callback(() =>
            {
                Console.WriteLine("B.SayName Invoke");
            });
            b.Object.SayName();

        }
        #region Test Classes

        public abstract class A
        {
            public virtual void SayName()
            {
                // Castle.Proxies.BProxy
                Console.WriteLine(this.GetType());
                Console.WriteLine("AAA");
            }
        }

        public class B : A
        {

        }


        // callbacks for methods with `ref` / `out` parameters are possible but require some work (and Moq 4.8 or later):
        delegate void SubmitCallback(ref Bar bar);

        public interface IFoo
        {
            Bar Bar { get; set; }
            string Name { get; set; }
            int Value { get; set; }
            bool DoSomething(string value);
            bool DoSomething(int number, string value);
            string DoSomethingStringy(string value);
            bool TryParse(string value, out string outputValue);
            bool Submit(ref Bar bar);
            int GetCount();
            bool Add(int value);
        }

        public class Bar
        {
            public virtual Baz Baz
            {
                get;
                set;
            }
            public virtual bool Submit()
            {
                return false;
            }
        }

        public class Baz
        {
            public virtual string Name
            {
                get;
                set;
            }
        }

        public static TValue IsAny<TValue>()
        {
            return Match<TValue>.Create(
                value => value == null || typeof(TValue).IsAssignableFrom(value.GetType()),
                () => It.IsAny<TValue>());
        }

        #endregion
    }

}
