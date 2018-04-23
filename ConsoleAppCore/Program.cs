﻿using ConsoleAppCore.Demos;
using ConsoleAppCore.Util;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ConsoleAppCore.MyLinq;
using ConsoleAppCore.MyLinq.SimpleVisitor;
using ConsoleAppCore.MyLinq.LinqToTerra;

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
            //MyLinqTest.Run();
            //SimpleVisitorTest.Run();
            LinqToTerraTest.Run();
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