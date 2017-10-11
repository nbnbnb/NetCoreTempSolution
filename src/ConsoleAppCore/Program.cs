using MyLibraryStandard;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //Demos.FileWatch();
            Temp();
            Console.ReadKey(true);
        }

        static void Temp()
        {
            Console.WriteLine("----- Start -----");
            Console.WriteLine($"LibraryCore The answer is {new Thing().Get(42)}.");
            Console.ReadKey(false);
            Console.WriteLine("-----  End  -----");
        }
    }


}