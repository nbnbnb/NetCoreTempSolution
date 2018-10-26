﻿using System;
using System.Collections.Generic;
using System.Reflection;
using ConsoleAppCore.Demos;
using ConsoleAppCore.Demos.SimpleAOP;

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
            Demos.AspectCore.AspectCoreRunner.Run();
        }
    }
}