using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.HarmonyDemo.Prefix
{

    // 此处指定了需要 Patch 的方法名 GetName
    [HarmonyPatch(typeof(OriginalCode), "GetName")]
    class Patch01
    {
        // 签名是 static bool
        // 默认方法名 Prefix
        // 如果不是通过注解方式使用，则反射方法的时候，
        static bool Prefix(ref string __result)
        {
            Console.WriteLine("Patch01 Prefix");

            // __result 这种带两个下划线的变量，是特殊的格式
            // 表示原来方法的返回值
            // 此处，还用 ref 进行了修饰

            // 设置 GetName 方法的返回值
            __result = __result + "Result: 001";

            // true：继续执行原始方法
            // 如果继续执行了原始方法，那么这里设置的 __result 是没有效果的
            // 因为原始方法会返回一个新值

            // false：不再执行原始方法（也不会执行 Patch03 的 Prefix 方法了）
            return false;
        }


        static void Postfix()
        {
            Console.WriteLine("Patch01 Postfix");
        }
    }
}
