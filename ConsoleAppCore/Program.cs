using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleAppCore.Demos;
using ConsoleAppCore.Demos.Misc;
using ConsoleAppCore.Demos.SimpleAOP;
using ConsoleAppCore.Util;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using System.Xml.Serialization;
using System.Text;
using System.Net.Http;
using ConsoleAppCore.MyLinq.LinqToTerra;
using ConsoleAppCore.Demos.CSharp80;
using ConsoleAppCore.Demos.AspectCore;
using ConsoleAppCore.Demos.HarmonyDemo.Prefix;
using ConsoleAppCore.Demos.HarmonyDemo.Reverse;

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
         
        }

    }
}