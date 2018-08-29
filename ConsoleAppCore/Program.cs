using System;
using ConsoleAppCore.Demos;

namespace ConsoleAppCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("----- Start -----");
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }
        private static void Temp()
        {
            Runner.MediatR();
        }
    }
}