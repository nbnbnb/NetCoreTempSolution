using System;
using System.Collections.Generic;
using System.Reflection;
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

            // 使用 [HarmonyPatch(typeof(OriginalCode), "GetName")] 注解方法
            // 如果一个方法上有多个 Patch，如何控制顺序？？？（经测试 Patch01 类在 Patch03 类前面，Patch 类在 Patch01 类前面）

            // 使用 Patch 方法，显示调用，是可以控制这周顺序的
            var originalCode = new OriginalCode();
            var name = originalCode.GetName();

            Console.WriteLine(name);

        }

        public static void RunWithOrder()
        {
            var harmony = new Harmony("ml.zhangjin");

            var original = typeof(OriginalCode).GetMethod("GetName");
            var prefix01 = typeof(Patch01).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static);
            var postfix01 = typeof(Patch01).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);
            var prefix02 = typeof(Patch02).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static);
            var postfix02 = typeof(Patch02).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);
            var prefix03 = typeof(Patch03).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static);
            var postfix03 = typeof(Patch03).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);

            // 指定不同的顺序
            harmony.Patch(original, new HarmonyMethod(prefix03), new HarmonyMethod(postfix03));
            harmony.Patch(original, new HarmonyMethod(prefix02), new HarmonyMethod(postfix02));
            harmony.Patch(original, new HarmonyMethod(prefix01), new HarmonyMethod(postfix01));

            var originalCode = new OriginalCode();
            var name = originalCode.GetName();

            Console.WriteLine(name);
        }

    }
}
