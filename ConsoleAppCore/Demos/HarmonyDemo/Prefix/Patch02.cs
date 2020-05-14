using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Demos.HarmonyDemo.Prefix
{

    [HarmonyPatch(typeof(OriginalCode), "GetName")]
    class Patch02
    {
        // this example uses a Stopwatch type to measure
        // and share state between prefix and postfix
        // out 引用方式，执行变量传递
        static void Prefix(out Stopwatch __state)
        {
            Console.WriteLine("Patch02 Prefix");
            __state = new Stopwatch(); // assign your own state
            __state.Start();
        }

        static void Postfix(Stopwatch __state)
        {
            Thread.Sleep(123);
            __state.Stop();
            Console.WriteLine($"ElapsedMilliseconds : {__state.ElapsedMilliseconds}");
            Console.WriteLine("Patch02 Postfix");
        }
    }
}
