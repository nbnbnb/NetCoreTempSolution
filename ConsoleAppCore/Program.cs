using ConsoleAppCore.Demos;
using ConsoleAppCore.Util;
using System;

namespace ConsoleAppCore
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("----- Start -----");
            //TheDemos();
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }

        static void Temp()
        {
            CSharp70Features.UseRef();
        }

        static void TheDemos(String[] args)
        {
            Runner.FileWatch();
            Runner.ReadCommandLineArgs();
            Runner.ReadEFConfig();
            Runner.ShowTaskLogger().NoWarning();
            Runner.ShowAppDomainExceptions();
            Runner.LockerDiff();
        }
    }
}