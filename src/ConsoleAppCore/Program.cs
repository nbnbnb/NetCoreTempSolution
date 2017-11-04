using ConsoleAppCore.Extensions;
using ConsoleAppCore.Util;
using MyLibraryStandard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ConsoleAppCore
{
    class Program
    {
        static async Task Main(String[] args)
        {
            var result = Enumerable.Range(1, 40)
                .Select(input => (input, task: FibonacciAsync(input)))
                .ToArray();

            Console.WriteLine("GOGOGO");

            foreach (var tuple in result)
            {
                Console.WriteLine($"Fib {tuple.input} =  {await tuple.task}");
            }

            Console.WriteLine("----- Start -----");
            //TheDemos(args);
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }

        private static Task<int> FibonacciAsync(int n, CancellationToken token = default)
        {
            return Task.Run(() => Fib(n).curr, token);

            (int curr, int prev) Fib(int i)
            {
                if (i is 0)
                {
                    return (1, 0);
                }
                var (c, p) = Fib(i - 1);
                return (c + p, c);
            }
        }

        static void Temp()
        {

        }

        static void TheDemos(String[] args)
        {
            Runner.FileWatch();
            Runner.LibraryStandard();
            Runner.ReadCommandLineArgs(args);
            Runner.ReadEFConfig();
            Runner.ShowTaskLogger().NoWarning();
            Runner.ShowAppDomainExceptions();
        }
    }
}