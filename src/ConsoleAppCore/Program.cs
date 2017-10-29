using MyLibraryStandard;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("----- Start -----");
            Demo(args);
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }

        static void Temp()
        {

        }

        static void Demo(String[] args)
        {
            Demos.FileWatch();
            Demos.LibraryStandard();
            Demos.ReadCommandLineArgs(args);
            Demos.ReadEFConfig();
        }
    }


}