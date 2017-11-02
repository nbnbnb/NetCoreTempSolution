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

        static bool DoTest()
        {
            List<Int32> abc = new List<int> { 1, 2, 3, 4, 5, 6 };

            abc.ForEach(m =>
            {
                if (m == 2)
                {
                    return true;
                }

                Console.WriteLine(m);
            });

            return false;
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