using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.HarmonyDemo.Prefix
{

    // 此处指定了需要 Patch 的方法名 GetName
    [HarmonyPatch(typeof(OriginalCode), "GetName")]
    class Patch
    {
        static bool Prefix(ref string __result)
        {
            // __result 这种带两个下划线的变量，是特殊的格式
            // 表示原来方法的返回值
            // 此处，还用 ref 进行了修饰

            // 设置 GetName 方法的返回值
            __result = "test";

            // true：跳过原始的 GetName 方法逻辑
            // false：继续执行原来的方法
            return true;
        }
    }
}
