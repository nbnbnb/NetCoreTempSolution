using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.HarmonyDemo.Prefix
{

    // 此处指定了需要 Patch 的方法名 GetName
    [HarmonyPatch(typeof(OriginalCode), "GetName")]
    class Patch03
    {
        // 签名是 static bool
        // 默认方法名 Prefix
        static bool Prefix(ref string __result)
        {
            Console.WriteLine("Patch03 Prefix");

            // __result 这种带两个下划线的变量，是特殊的格式
            // 表示原来方法的返回值
            // 此处，还用 ref 进行了修饰

            // 设置 GetName 方法的返回值
            __result = __result + "Result: 003";

            // true：继续执行原始方法
            // 如果继续执行了原始方法，那么这里设置的 __result 是没有效果的
            // 因为原始方法会返回一个新值

            // false：不再执行原始方法
            return false;
        }

        static void Postfix()
        {
            Console.WriteLine("Patch03 Postfix");
        }
    }
}
