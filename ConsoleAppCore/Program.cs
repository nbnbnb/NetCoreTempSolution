﻿using ConsoleAppCore.Demos;
using System;

namespace ConsoleAppCore
{

    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("----- Start -----");
            Temp();
            Console.WriteLine("-----  End  -----");
            Console.WriteLine();
            Console.WriteLine("按任意键退出");
            Console.ReadKey(true);
        }

        static void Temp()
        {
            CSharp73Features.M();
            CSharp73Features.M2();
        }
    }
}