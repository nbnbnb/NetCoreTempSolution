using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace ConsoleAppCore.Demos.HarmonyDemo.Prefix
{
    class PrefixRunner
    {
        public static void Run()
        {
            var harmony = new Harmony("ml.zhangjin");
            harmony.PatchAll();

            var originalCode = new OriginalCode();
            var name = originalCode.GetName();

            Console.WriteLine(name);

        }
    }
}
