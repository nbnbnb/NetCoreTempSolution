using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using ConsoleAppCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using ConsoleAppCore.Util;
using System.Threading;
using System.Runtime.ExceptionServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ConsoleAppCore.Demos.Locker;

namespace ConsoleAppCore
{
    static class Runner
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void M()
        {

        }

        private static Dictionary<string, string> GetSwitchMappings(IReadOnlyDictionary<string, string> configurationStrings)
        {
            /*
             *  将键和值进行转换
                Key: "-MachineName"
                Value: "Profile:MachineName"
                Key: "-Left"
                Value: "App:MainWindow:Left"
            */

            return configurationStrings
                .Select(item => new KeyValuePair<string, string>("-" + item.Key.Substring(item.Key.LastIndexOf(':') + 1), item.Key))
                .ToDictionary(item => item.Key, item => item.Value);
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
        /// 读取 .NET Standard 库
        /// </summary>
        public static void LibraryStandard()
        {
            Console.WriteLine($"LibraryCore The answer is {new MyLibraryStandard.Thing().Get(42)}.");
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
        public static void ReadCommandLineArgs(String[] args)
        {
            // 支持格式
            // dotnet run -MachineName=Bob -Left=7734
            // dotnet run /Profile:MachineName=  # 设置空值，并且使用 / 前导，表示直接指定值，不需要映射表

            var dict = new Dictionary<string, string>
            {
                {"Profile:MachineName", "Rick"},
                {"App:MainWindow:Left", "11"}
            };

            var builder = new ConfigurationBuilder();
            builder
                .AddInMemoryCollection(dict)
                .AddCommandLine(args, GetSwitchMappings(dict));   //  用命令行参数值覆原始值，需要一个 Key-Value 的映射关系

            IConfigurationRoot _config = builder.Build();

            Console.WriteLine($"Hello {_config["Profile:MachineName"]}");

            // Set the default value to 80
            var left = _config.GetValue<int>("App:MainWindow:Left", 80);
            Console.WriteLine($"Left {left}");
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
                // 使用 dotnet run 命令和 VS 运行时，监听的是"项目根"目录 的 quotes.txt
                // 使用 dotnet ConsoleAppCore.dll 命令运行时，监听的是"ConsoleAppCore.dll 同级"目录下的 quotes.txt
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
            }

            foreach (var op in TaskLogger.GetLogEntries().OrderBy(m => m.LogTime))
            {
                Console.WriteLine(op);
            }
        }

        /// <summary>
        /// 通过　FirstChanceException 捕获　AppDomain 中的异常
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
                catch (Exception)
                {
                    // 在 FirstChanceException 中通知
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
    }
}
