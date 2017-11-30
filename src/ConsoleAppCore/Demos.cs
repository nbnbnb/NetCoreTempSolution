﻿using System;
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

namespace ConsoleAppCore
{
    static class Demos
    {
        private static Dictionary<string, string> GetSwitchMappings(IReadOnlyDictionary<string, string> configurationStrings)
        {
            /*
             *  将键和值进行转换
                Key: "-MachineName" Value: "Profile:MachineName"
                Key: "-Left" Value: "App:MainWindow:Left"
            */

            var part1 = configurationStrings.Select(item => new KeyValuePair<string, string>("-" + item.Key.Substring(item.Key.LastIndexOf(':') + 1), item.Key));

            var part2 = part1.ToDictionary(item => item.Key, item => item.Value);

            return part2;

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
        public static void ReadCommandLineArgs()
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
            builder.AddInMemoryCollection(dict);  // 获取内存中（自定义）的键值对                                                         
            IConfigurationRoot _config = builder.Build();

            // 读取内存中的配置
            Console.WriteLine(_config["Profile:MachineName"]);  // Rick 
            // 读取内存中的配置
            // 使用自动类型转换（基本类型），并指定默认值
            Console.WriteLine(_config.GetValue<int>("App:MainWindow:Left", -1)); // 11 

            // 用命令行参数覆盖内存参数
            // 命令行支持的格式为
            // "/" 格式，全匹配 /Profile:MachineName=ZhangJin-PC
            // "-" 格式，后缀匹配 -MachineName=ZhangJin-PC
            // 是通过 GetSwitchMappings 方法进行转换的

            /*
             *  GetSwitchMappings 将键和值进行转换
                Key: "-MachineName" Value: "Profile:MachineName"
                Key: "-Left"        Value: "App:MainWindow:Left"
            */

            String[] args = new[] { "/Profile:MachineName=ZhhangJin-PC", "-Left=9800" };

            Dictionary<String, String> mapping = new
            {

            };

            builder.AddCommandLine(args, GetSwitchMappings(dict));   //  用命令行参数值覆原始值，需要一个 Key-Value 的映射关系
            _config = builder.Build();

            Console.WriteLine("-------------");

            Console.WriteLine(_config["Profile:MachineName"]);  // Bob
            Console.WriteLine(_config.GetValue<int>("App:MainWindow:Left", -1));  // 
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
    }
}
