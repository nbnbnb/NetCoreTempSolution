using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.HarmonyDemo.Reverse
{
    class ReverseRunner
    {
        public static void Run()
        {
            /**
             * 使用场景
             * 
             * 1，方便的调用私有方法
             * 2，不使用反射，委托原始性能
             * 3，可以使用一个 Transplier 来修改原始方法，或提取其中的一部分方法
             * 4，访问原始未修改的 IL
             * 5，
             * */

            var harmony = new Harmony("ml.zhangjin");
            harmony.PatchAll();

            var originalInstance = new OriginalCode02();

            // 将请求转发到 OriginalCode02.Test() 方法中
            // 这是一个私有方法，也是可以调用的

            // Patch03.MyTest() 的签名要和 OriginalCode02.Test() 保存一直
            // 如果代理的是实例方法，则需要第一个参数 originalInstance
            // 如果是静态方法，则不需要
            Patch03.MyTest(originalInstance, 100, "hello");

           
        }
    }
}
