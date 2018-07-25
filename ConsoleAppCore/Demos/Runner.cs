using ConsoleAppCore.Demos.Emit;
using ConsoleAppCore.Demos.Locker;
using ConsoleAppCore.Demos.MediatR;
using ConsoleAppCore.Extensions;
using ConsoleAppCore.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings.Resolvers;
using MediatR.Pipeline;
using Ninject.Syntax;
using MediatR;

namespace ConsoleAppCore
{
    static class Runner
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void M()
        {

        }

        /// <summary>
        /// 在 AppDomain 中注册 FirstChanceException 事件
        /// </summary>
        private static async void RegisterExceptioin()
        {
            var eventAwaiter = new EventAwaiter<FirstChanceExceptionEventArgs>();
            AppDomain.CurrentDomain.FirstChanceException += eventAwaiter.EventRaised;
            while (true)
            {
                // 每个异常都是通过 ContinueWith 进行处理的
                Console.WriteLine("AppDomain exception: {0}", (await eventAwaiter).Exception.GetType());
            }
        }

        /// <summary>
        /// 从命令行读取参数信息
        /// 支持 -XXX 和 /XXX 格式
        /// 指定参数覆盖默认参数
        /// 
        /// 添加包 Microsoft.Extensions.Configuration
        /// 添加包 Microsoft.Extensions.Options.ConfigurationExtensions
        /// 添加包 Microsoft.Extensions.Configuration.Json
        /// 添加包 Microsoft.Extensions.Configuration.Binder
        /// 添加包 Microsoft.Extensions.Configuration.CommandLine
        /// </summary>
        public static void ReadCommandLineArgs()
        {
            var dict = new Dictionary<string, string>
            {
                {"Profile:MachineName", "Rick"},
                {"App:MainWindow:Left", "11"},
                {"App:MainWindow:Right", "54"}
            };

            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(dict);  // 获取内存中（自定义）的键值对                                                         
            IConfigurationRoot _config = builder.Build();

            // 读取内存中的配置
            Console.WriteLine(_config["Profile:MachineName"]);  // Rick 
            // 读取内存中的配置
            // 使用自动类型转换（基本类型），并指定默认值
            Console.WriteLine(_config.GetValue<int>("App:MainWindow:Left", -1)); // 11 
            Console.WriteLine(_config.GetValue<int>("App:MainWindow:Right", -1)); // 54

            // 支持格式
            // dotnet run /Profile:MachineName=ZhangJin-PC /Profile:Address=Home -Left=7734 -Left=9800

            // Command Line Args
            // "/" 格式的不需要 Mapping，进行全匹配
            // "-" 格式的需要 Mapping
            // 模拟命令行参数
            String[] args = new[] {
                "/Profile:MachineName=ZhangJin-PC",
                "-ExchangeLeft=9800",  // 和交互字典里面的键匹配  // 以 - 开头
                "--ExchangeRight=900"  // 和交互字典里面的键匹配  // 以 -- 开头
            };

            // 交互字典

            // 交互字典的规则
            // 1，交换必须以单划线 (-) 或双划线 (--) 开头
            // 2，交换映射字典不得包含重复键
            Dictionary<String, String> mapping = new Dictionary<string, string>
            {
                // 命令行参数要和 mapping Key 匹配
                // Key 匹配成功后，把 Value 当作应用内部使用的键，args 中获取的值当作应用内部键的值
                { "-ExchangeLeft","App:MainWindow:Left"},  // 以 - 开头
                { "--ExchangeRight","App:MainWindow:Right"}, // 以 -- 开头
                { "-Test","TestKey"}, // 多出的一个 mapping 键，使用控制台参数时可以不传递，程序中获得的 TestKey 为 null
            };

            // "-" 格式的 args 不能比 mapping 多
            // 第二个参数 mapping 是交换字典
            builder.AddCommandLine(args, mapping);   //  用命令行参数值覆原始值，需要一个 Key-Value 的映射关系
            _config = builder.Build();

            Console.WriteLine("-------------------------------");

            Console.WriteLine(_config["Profile:MachineName"]);  // ZhangJin-PC
            Console.WriteLine(_config.GetValue<int>("App:MainWindow:Left", -1));  //  9800
            Console.WriteLine(_config.GetValue<int>("App:MainWindow:Right", -1));  //  900
            Console.WriteLine(_config.GetValue("TestKey", "The Test Key Default Value"));  //  "The Test Key Default Value"
        }

        /// <summary>
        /// 读取 appsettings.json 中的 key-value 信息
        /// </summary>
        public static void ReadEFConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var connectionStringConfig = builder.Build();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // Add "appsettings.json" to bootstrap EF config.
                .AddJsonFile("appsettings.json")
                // Add the EF configuration provider, which will override any
                // config made with the JSON provider.
                .AddEntityFrameworkConfig(options => options.UseSqlServer(connectionStringConfig.GetConnectionString("DefaultConnection")))
                .Build();

            Console.WriteLine("key1={0}", config["key1"]);
            Console.WriteLine("key2={0}", config["key2"]);
            Console.WriteLine("key3={0}", config["key3"]);
        }

        /// <summary>
        /// 监听文件变动
        /// Debug 时，修改 "解决方案" /Resource 目录下的 quotes.txt 查看变动
        /// </summary>
        public static void FileWatch()
        {
            Console.WriteLine("Monitoring quotes.txt for changes (ctrl-c to quit)...");

            Console.WriteLine($"Current Directory Is {Directory.GetCurrentDirectory()}");

            PhysicalFileProvider fileProvider =
                new PhysicalFileProvider(Directory.GetCurrentDirectory());

            async Task watch()
            {
                // 每次都需要创建一个新的 Token

                // 使用 dotnet run 命令和 VS 运行时，监听的是 "解决方案" /Resource 目录下的 quotes.txt
                // 使用 dotnet ConsoleAppCore.dll 命令运行时，监听的是 "ConsoleAppCore.dll 同级" /Resource 目录下的 quotes.txt

                // 使用 Globbing patterns 进行匹配
                IChangeToken token = fileProvider.Watch("Resource/quotes.txt");

                var tcs = new TaskCompletionSource<object>();
                token.RegisterChangeCallback(state =>
                {
                    ((TaskCompletionSource<object>)state).TrySetResult(null);
                }, tcs);

                await tcs.Task.ConfigureAwait(false);
                Console.WriteLine("quotes.txt changed.");
            };

            Task.Run(() =>
            {
                while (true)
                {
                    watch().GetAwaiter().GetResult();
                }
            });
        }

        /// <summary>
        /// 显示尚未完成的异步操作
        /// 
        /// 同时内部使用帮助方法 WithCancellation 用于串联协作式取消
        /// </summary>
        public static async Task ShowTaskLogger()
        {
#if DEBUG
            TaskLogger.LogLevel = TaskLogger.TaskLogLevel.Pending;
#endif
            var tasks = new List<Task>
            {
                   Task.Delay(2000).Log("2s op"),
                   Task.Delay(5000).Log("5s op"),
                   Task.Delay(6000).Log("6s op")
            };

            try
            {
                await Task.WhenAll(tasks).WithCancellation(new CancellationTokenSource(3000).Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Task is canceled");
            }

            foreach (var op in TaskLogger.GetLogEntries().OrderBy(m => m.LogTime))
            {
                Console.WriteLine(op);
            }
        }

        /// <summary>
        /// 通过　FirstChanceException 捕获　AppDomain 中的异常
        /// 然后通过一个死循环的 awaitr 处理异常信息        
        /// </summary>
        public static void ShowAppDomainExceptions()
        {
            RegisterExceptioin();

            for (Int32 x = 0; x < 3; x++)
            {
                try
                {
                    switch (x)
                    {
                        case 0: throw new InvalidOperationException();
                        case 1: throw new ObjectDisposedException("");
                        case 2: throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Show Loop-Awaiter Test - {0}", ex.GetType());
                }
            }
        }

        /// <summary>
        /// 理解用户模式构造和内核模式构造的性能差异
        /// </summary>
        public static void LockerDiff()
        {
            Int32 x = 0;
            const Int32 iterations = 10_000_000;

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (Int32 i = 0; i < iterations; i++)
            {
                x++;
            }
            Console.WriteLine("Incrementing x: {0}", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                M();
                x++;
                M();
            }
            Console.WriteLine("Incrementing x in M: {0}", stopwatch.ElapsedMilliseconds);

            SpinLock spinLock = new SpinLock(false);
            stopwatch.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                Boolean taken = false;
                spinLock.Enter(ref taken);
                x++;
                spinLock.Exit();
            }
            Console.WriteLine("Incrementing x in SpinLock: {0}", stopwatch.ElapsedMilliseconds);

            SimpleSpinLock simpleSpinLock = new SimpleSpinLock();
            stopwatch.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                simpleSpinLock.Enter();
                x++;
                simpleSpinLock.Leave();
            }
            Console.WriteLine("Incrementing x in SimpleSpinLock: {0}", stopwatch.ElapsedMilliseconds);

            SimpleWaitLock simpleWaitLock = new SimpleWaitLock();
            stopwatch.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                simpleWaitLock.Enter();
                x++;
                simpleWaitLock.Leave();
            }
            Console.WriteLine("Incrementing x in SimpleWaitLock: {0}", stopwatch.ElapsedMilliseconds);

            SimpleHybridLock simpleHybridLock = new SimpleHybridLock();
            stopwatch.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                simpleHybridLock.Enter();
                x++;
                simpleHybridLock.Leave();
            }
            Console.WriteLine("Incrementing x in SimpleHybridLock: {0}", stopwatch.ElapsedMilliseconds);

            AnotherHybridLock anotherHybridLock = new AnotherHybridLock();
            stopwatch.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                anotherHybridLock.Enter();
                x++;
                anotherHybridLock.Leave();
            }
            Console.WriteLine("Incrementing x in AnotherHybridLock: {0}", stopwatch.ElapsedMilliseconds);
        }

        public static void CreateDynamicType()
        {
            //动态创建的类类型
            Type classType = BasicDynamicType.DynamicCreateType();
            //调用有参数的构造函数
            Type[] ctorTypes = new Type[] { typeof(string) };
            object[] ctorValues = new object[] { "Hello World" };
            ConstructorInfo ci = classType.GetConstructor(ctorTypes);
            object target = ci.Invoke(ctorValues);
            //调用方法
            object[] methedParams = new object[] { };
            Console.WriteLine(classType.InvokeMember("MyMethod", BindingFlags.InvokeMethod, null, target, methedParams));
        }

        public async static void MediatR()
        {
            // 有两种形式的 Request 类型
            // IRequest<T> 有返回值
            // IRequest 无返回值，IRequest 继承值 IRequest<Unit>

            // Request 对应的 Handler 接口类型（有返回值）
            // IRequestHandler<T, U>，implement this and return Task<U>.（异步）
            // RequestHandler<T, U>，inherit this and return U.（同步）

            // Request 对应的 Handler 接口类型（无返回值）
            // IRequestHandler<T> - implement this and you will return Task<Unit>.（异步）
            // AsyncRequestHandler<T> - inherit this and you will return Task.（异步）
            // RequestHandler<T> - inherit this and you will return nothing (void).（同步）

            var kernel = new StandardKernel();

            kernel.Bind(scan => scan.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());

            // Command 模式
            // 每个 Command 只对应一个 Handler
            // 将 ASyncPingHandler 和 IRequestHandler<Ping, string> 绑定
            // 将 SyncPongHandler 和 RequestHandler<Pong, string> 绑定
            // 将 AsyncOneWayHandler 和 AsyncRequestHandler<OneWay> 绑定
            kernel.Bind(scan => scan.FromThisAssembly().SelectAllClasses().InheritedFrom(typeof(IRequestHandler<,>)).BindAllInterfaces());

            // Pipeline
            // Publish 模式
            // 每个 Command 可以对应多个 Handler
            // 将 Ping01NotificationHandler 和 INotificationHandler<PingNotification> 绑定
            // 将 Ping02NotificationHandler 和 INotificationHandler<PingNotification> 绑定
            // 将 Ping03NotificationHandler 和 INotificationHandler<PingNotification> 绑定
            kernel.Bind(scan => scan.FromThisAssembly().SelectAllClasses().InheritedFrom(typeof(INotificationHandler<>)).BindAllInterfaces());

            // kernel.Bind(typeof(IPipelineBehavior<,>)).To(typeof(RequestPreProcessorBehavior<,>));
            // kernel.Bind(typeof(IPipelineBehavior<,>)).To(typeof(RequestPostProcessorBehavior<,>));

            kernel.Bind<ServiceFactory>().ToMethod(ctx => t => ctx.Kernel.TryGet(t));

            var mediator = kernel.Get<IMediator>();

            // 有返回值 Async Handler
            Console.WriteLine(await mediator.Send(new Ping() { MsgId = 100 }));

            // 有返回值 Sync Handler
            Console.WriteLine(await mediator.Send(new Pong() { MsgId = 100 }));

            // 无返回值 Async Handler
            await mediator.Send(new OneWay());

            // Publish
            await mediator.Publish(new PingNotification());
        }
    }
}
