using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Moq.Protected;
using System.Reflection;
using Match = Moq.Match;

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
            mock.Setup(m => m.Add(It.IsInRange<int>(0, 10, Moq.Range.Inclusive))).Returns(true);
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

            // 默认情况下，属性返回的是默认值
            mock = new Mock<IFoo> { DefaultValue = DefaultValue.Empty };
            Assert.AreEqual(mock.Object.Bar, null);
            // 可以设置为返回一个 Mock 对象
            mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };
            Assert.IsNotNull(mock.Object.Bar);
            // 并且从这个对象中，还可以获得 Mock 对象
            // 然后对这个对象进行相应的设置
            Mock<Bar> mockBar = Mock.Get(mock.Object.Bar);
            mockBar.Setup(m => m.Submit()).Returns(true);

            // 使用 MockRepository 将所有的 Behavior 同时设置
            var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

            // 使用 MockRepository 的设置
            var fooMock = repository.Create<IFoo>();

            // 可以覆盖设置
            var barMock = repository.Create<Bar>(MockBehavior.Loose);

            // 执行 MockRepository 上面的验证
            repository.Verify();

        }

        [TestMethod]
        public void Miscellaneous_Test()
        {
            var mock = new Mock<IFoo>();
            // 使用 SetupSequence 设置每次调用返回不同的值
            mock.SetupSequence(f => f.GetCount())
                .Returns(3)  // will be returned on 1st invocation
                .Returns(2)  // will be returned on 2nd invocation
                .Returns(1)  // will be returned on 3rd invocation
                .Returns(0)  // will be returned on 4th invocation
                .Throws(new InvalidOperationException());  // will be thrown on 5th invocation

            // in the test
            // using Moq.Protected;
            // 对于受保护的成员，无法通过智能提示访问其程序信息
            // 此处可以使用 Protected() 扩展方法，然后通过字符串的信息进行 Setup
            var mockBase = new Mock<CommandBase>();
            mockBase.Protected()
                 .Setup<int>("Execute")
                 .Returns(5);
            // 如果是传递参数的场景，需要使用 ItExpr，而不能使用 It
            mockBase.Protected()
                .Setup<bool>("Execute", ItExpr.IsAny<String>())
                .Returns(true);

            // Moq 4.8+ 支持使用一个兼容受保护成员的接口，来提供强类型支持
            mockBase.Protected().As<CommandBaseProtectedMembers>()
                .Setup(m => m.Show(It.IsAny<string>()))
                .Returns<String>(m => m);

            // 传递的 Mock 对象，覆盖了原始的实现
            CommandBase.ABC abc = new CommandBase.ABC(mockBase.Object);
            Assert.AreEqual(abc.ExecuteId, 5);
            Assert.IsTrue(abc.ExecuteBool);
            Assert.AreEqual(abc.ShowStr, "abc");
        }

        [TestMethod]
        public void Advanced_Features_Test()
        {
            Mock<IFoo> mock = new Mock<IFoo>();
            // 这个对象其实包含 Mock 信息
            IFoo foo = mock.Object;
            // 通过 Mock.Get 可以获得原始的 Mock 数据
            Mock<IFoo> fooMock = Mock.Get(foo);
            fooMock.Setup(f => f.GetCount()).Returns(42);
            Assert.AreEqual(foo.GetCount(), 42);


            Mock<IFoo> mock2 = new Mock<IFoo>();
            // 可以转型到接口进行 Mock
            Mock<IDisposable> disposableFoo = mock2.As<IDisposable>();
            // 然后可以设置接口的方法（不转型不能设置）
            disposableFoo.Setup(disposable => disposable.Dispose());
            // 在单个 mock 中设置
            mock2.As<IDisposable>().Setup(disposable => disposable.Dispose());

            // 自定义 Matcher
            mock2.Setup(m => m.DoSomething(IsSmall())).Throws<ArgumentException>();

            // 参数小于 5 个字符
            Assert.ThrowsException<ArgumentException>(() => mock2.Object.DoSomething("abc"));

            // 使用自定义的默认值提供程序
            var mock3 = new Mock<IFoo>
            {
                DefaultValueProvider = new MyEmptyDefaultValueProvider()
            };

            Assert.AreEqual("?", mock3.Object.Name);
            // 此时为 DefaultValue.Custom
            Assert.AreEqual(mock3.DefaultValue, DefaultValue.Custom);

        }

        #region Test Items

        /// <summary>
        /// 创建自定义的默认值提供程序
        /// </summary>
        class MyEmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
        {
            public MyEmptyDefaultValueProvider()
            {
                // 如果是字符串类型，则返回 ?
                base.Register(typeof(string), (type, mock) => "?");
                base.Register(typeof(List<>), (type, mock) => Activator.CreateInstance(type));
            }
        }

        public string IsSmall()
        {
            return Match.Create<string>(s => !String.IsNullOrEmpty(s) && s.Length < 5);
        }

        interface CommandBaseProtectedMembers
        {
            string Show(string arg);
        }

        public class CommandBase
        {
            // 访问级别不是 public
            // 此处 Moq 无法读取到 Execute 方法信息
            protected virtual int Execute()
            {
                return 0;
            }

            protected virtual bool Execute(string arg)
            {
                return false;
            }

            protected virtual string Show(string arg)
            {
                return arg;
            }

            // 由于 Execute 是 protected 级别
            // 此处放在嵌套类中可以访问
            public class ABC
            {
                public int ExecuteId { get; private set; }

                public bool ExecuteBool { get; private set; }

                public string ShowStr { get; private set; }

                public ABC(CommandBase command)
                {
                    ExecuteId = command.Execute();
                    ExecuteBool = command.Execute("xyz");
                    ShowStr = command.Show("abc");
                }
            }
        }

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
