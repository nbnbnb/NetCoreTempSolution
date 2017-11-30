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
using ConsoleAppCore.Demos;

namespace ConsoleAppCore
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("----- Start -----");
            //TheDemos(args);
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }

        static void Temp()
        {
            Runner.ReadCommandLineArgs();
        }

        static void TheDemos(String[] args)
        {
            Runner.FileWatch();
            Runner.LibraryStandard();
            Runner.ReadCommandLineArgs();
            Runner.ReadEFConfig();
            Runner.ShowTaskLogger().NoWarning();
            Runner.ShowAppDomainExceptions();
            Runner.LockerDiff();
        }
    }
}