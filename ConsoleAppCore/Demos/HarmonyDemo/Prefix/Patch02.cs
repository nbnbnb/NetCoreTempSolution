using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsoleAppCore.Demos.HarmonyDemo.Prefix
{

    [HarmonyPatch(typeof(OriginalCode), "GetName")]
    class Patch02
    {
        // this example uses a Stopwatch type to measure
        // and share state between prefix and postfix

        static void Prefix(out Stopwatch __state)
        {
            Console.WriteLine("Stopwatch Prefix");
            __state = new Stopwatch(); // assign your own state
            __state.Start();
        }

        static void Postfix(Stopwatch __state)
        {
            __state.Stop();
            Console.WriteLine($"ElapsedMilliseconds : {__state.ElapsedMilliseconds}");
            Console.WriteLine("Stopwatch Postfix");
        }
    }
}
