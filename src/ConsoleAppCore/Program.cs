using ConsoleAppCore.Extensions;
using ConsoleAppCore.Util;
using MyLibraryStandard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        }

        static void TheDemos(String[] args)
        {
            Demos.FileWatch();
            Demos.LibraryStandard();
            Demos.ReadCommandLineArgs(args);
            Demos.ReadEFConfig();
            Demos.ShowTaskLogger().NoWarning();
            Demos.ShowAppDomainExceptions();
        }
    }
}