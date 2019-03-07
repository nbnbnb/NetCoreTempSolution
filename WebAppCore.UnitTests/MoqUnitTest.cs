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
            //输入指定的参数，返回指定的结果
            mock.Setup(m => m.DoSomething("ping")).Returns(true);

            var outString = "ack";
            // 使用 out 参数（在内部初始化）
            // TryParse will return true, and the out argument will return "ack", lazy evaluated
            mock.Setup(m => m.TryParse("ping", out outString)).Returns(true);

            var bar = new Bar();
            var bar2 = new Bar();
            // 使用 ref 参数（在外部初始化）
            // Only matches if the ref argument to the invocation is the same instance
            mock.Setup(m => m.Submit(ref bar)).Returns(true);
            // Setup 的参数和验证的参数要保证是同一个实例
            Assert.IsTrue(fakeFoo.Submit(ref bar));
            // 如果是不同的实例，返回 false
            Assert.IsFalse(fakeFoo.Submit(ref bar2));

            // 调用方法，返回值
            // 使用 It.IsAny<T> 模拟任何参数
            mock.Setup(m => m.DoSomethingStringy(It.IsAny<String>()))
                .Returns<String>(s => s.ToLower());

            // 验证抛出异常
            mock.Setup(m => m.DoSomething("reset")).Throws<InvalidOperationException>();
            Assert.ThrowsException<InvalidOperationException>(() => fakeFoo.DoSomething("reset"));

            // 延迟初始化返回值
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

            // Func<T,bool> 匹配
            mock.Setup(m => m.Add(It.Is<int>(i => i % 2 == 0))).Returns(true);
            Assert.IsFalse(fakeFoo.Add(1));
            Assert.IsTrue(fakeFoo.Add(2));
            // Range 匹配
            mock.Setup(m => m.Add(It.IsInRange<int>(0, 10, Range.Inclusive))).Returns(true);
            Assert.IsTrue(fakeFoo.Add(9));
            // Regex 匹配
            mock.Setup(m => m.DoSomethingStringy(It.IsRegex("[a-d]+", RegexOptions.IgnoreCase))).Returns("abcd");
            Assert.IsTrue(fakeFoo.DoSomethingStringy("abcd") == "abcd");
            Assert.IsTrue(fakeFoo.DoSomethingStringy("abcd") == "abcd");
        }

        [TestMethod]
        public void Properties_Test()
        {
            var mock = new Mock<IFoo>();
            var fakeFoo = mock.Object;

            // 导航属性赋值
            mock.Setup(m => m.Bar.Baz.Name).Returns("baz");
            Assert.IsTrue(fakeFoo.Bar.Baz.Name == "baz");

            // 期望一个属性被设置对应的值
            mock.SetupSet(m => m.Name = "foo");
            // 执行期望的赋值
            fakeFoo.Name = "foo";
            // 最后验证这个赋值是否成功
            mock.VerifySet(m => m.Name = "foo");
            //mock.VerifyAll();

            // 设置属性有对应的 property behavior
            // 如果不这么设置，则是无法进行赋值的
            // 例如
            fakeFoo.Name = "abc";
            // 赋值没有成功
            Assert.IsTrue(fakeFoo.Name == null);
            // 使用 SetupProperty 打开赋值功能
            // 默认值为 null
            mock.SetupProperty(m => m.Name);
            Assert.AreEqual(fakeFoo.Name, null);
            // 设置未赋值时的默认值
            mock.SetupProperty(m => m.Name, "foo");
            Assert.AreEqual("foo", fakeFoo.Name);
            // 重新设置值
            fakeFoo.Name = "bar";
            Assert.AreEqual("bar", fakeFoo.Name);

            // 对所有的属性设置 property behavior
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
                // 在方法执行的时候执行这个回调
                .Callback(() => calls++)
                .Returns(true);

            mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
                // Callback 中接收方法参数
                // 非泛型
                .Callback((string s) => callArgs.Add(s))
                .Returns(true);

            // Callback 中接收方法参数
            // 泛型
            mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
                .Callback<string>(s => callArgs.Add(s))
             .Returns(true);

            // 多个参数
            mock.Setup(foo => foo.DoSomething(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((i, s) => callArgs.Add(s))
                .Returns(true);

            // 前后调用
            mock.Setup(foo => foo.DoSomething("ping"))
                .Callback(() => Console.WriteLine("Before returns"))
                .Returns(true)
                .Callback(() => Console.WriteLine("After returns"));

            // 使用 ref/out 参数时
            // 需要使用一个委托捕获这个参数
            mock.Setup(foo => foo.Submit(ref It.Ref<Bar>.IsAny))
                .Callback(new SubmitCallback((ref Bar bar) => Console.WriteLine("Submitting a Bar!")));

            // 小技巧
            // 每次调用返回不同的值
            mock.Setup(foo => foo.GetCount())
            .Callback(() => calls++)
            .Returns(() => calls);

        }

        [TestMethod]
        public void Behavior_Test()
        {
            // 默认是 Loose 模式
            // 当返回未 Setup 的属性时，返回其默认值
            // 当返回未 Setup 的方法时，返回其默认值或 void
            // 集合返回为元素为 0 的集合
            var mock = new Mock<IFoo>(MockBehavior.Default);
            var name = mock.Object.Name;
            Assert.IsFalse(mock.Object.DoSomething("abc"));
            // 换成 Strict 模式后，将会报错
            mock = new Mock<IFoo>(MockBehavior.Strict);
            Assert.ThrowsException<MockException>(() => name = mock.Object.Name);
            Assert.ThrowsException<MockException>(() => mock.Object.DoSomething("abc"));

            // Mock 类是否要调用基类的虚拟方法
            // 默认为 False
            // 对于模拟 System.Web 中的 Web/Html 这类控件，需要重写这些方法
            var b = new Mock<B> { CallBase = true };
            // 将会调用父类 A 类的 SayName 方法
            b.Object.SayName();
            Console.WriteLine("---");
            // 如果 Setup 了方法
            // 则不会调用父类的方法
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
