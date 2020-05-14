using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.HarmonyDemo.Reverse
{
    [HarmonyPatch]
    class Patch03
    {
        // 设置 [HarmonyReversePatch] 标记

        /*
         * 可以设置这两个标记
         * [HarmonyReversePatch(HarmonyReversePatchType.Original)]
           [HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]

           默认是 Original，表示原始的方法不会被 Patch/Transpilers

           Snapshot 表示所有的 Transpilers 会生效，Patch（Prefix/Postfix/Finalizer）不会生效

           ReversePatch 最强大的功能就是使用 Transpiler 修改原始方法的 IL
         * 
         **/

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(OriginalCode02), "Test")]
        public static void MyTest(object instance, int counter, string name)
        {
            // 这里的代码实际上是不会执行的
            // 在运行时，将直接调用 OriginalCode02.Test() 

            // 如果代码签名里面有返回值，可以返回默认值，保证编译通过即可
            // 实际返回的还是原始方法的执行结果

            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub");
        }

    }
}
