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

namespace ConsoleAppCore
{
    static class Demos
    {




        public static Dictionary<string, string> GetSwitchMappings(IReadOnlyDictionary<string, string> configurationStrings)
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
        /// 读取 .NET Standard
        /// </summary>
        public static void LibraryStandard()
        {
            Console.WriteLine($"LibraryCore The answer is {new MyLibraryStandard.Thing().Get(42)}.");
        }

        /// <summary>
        /// 读取控制参数配置中
        /// 添加包 Microsoft.Extensions.Configuration
        /// 添加包 Microsoft.Extensions.Options.ConfigurationExtensions
        /// 添加包 Microsoft.Extensions.Configuration.Json
        /// 添加包 Microsoft.Extensions.Configuration.Binder
        /// 添加包 Microsoft.Extensions.Configuration.CommandLine
        /// </summary>
        public static void ReadCommandLineArgs(string[] args)
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

            IConfigurationRoot _config = builder.Build(); ;

            Console.WriteLine($"Hello {_config["Profile:MachineName"]}");

            // Set the default value to 80
            var left = _config.GetValue<int>("App:MainWindow:Left", 80);
            Console.WriteLine($"Left {left}");
        }

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

        public static void FileWatch()
        {
            Console.WriteLine("Monitoring quotes.txt for changes (ctrl-c to quit)...");

            Console.WriteLine($"Current Directory Is {Directory.GetCurrentDirectory()}");

            PhysicalFileProvider fileProvider =
                new PhysicalFileProvider(Directory.GetCurrentDirectory());

            async Task watch()
            {
                // 每次都需要创建一个新的 Token
                // 使用 dotnet run 命令运行是，监听的是项目根目录下的 quotes.txt
                // 使用 dotnet ConsoleAppCore.dll 运行时，监听的是应用程序根目录下的 quotes.txt
                IChangeToken token = fileProvider.Watch("quotes.txt");   
                var tcs = new TaskCompletionSource<object>();
                token.RegisterChangeCallback(state =>
                {
                    ((TaskCompletionSource<object>)state).TrySetResult(null);
                }, tcs);
                await tcs.Task.ConfigureAwait(false);
                Console.WriteLine("quotes.txt changed.");
            };

            while (true)
            {
                watch().GetAwaiter().GetResult();
            }
        }

    }
}
