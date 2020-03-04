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

            // TODO
            // 如果一个方法上有多个 Patch，如果控制顺序？？？
            var originalCode = new OriginalCode();
            var name = originalCode.GetName();

            Console.WriteLine(name);

        }
    }
}
