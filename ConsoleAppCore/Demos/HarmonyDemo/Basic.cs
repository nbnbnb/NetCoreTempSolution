using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;

namespace ConsoleAppCore.Demos.HarmonyDemo
{
    public class Basic
    {
        public static void BasicUse()
        {
            // 创建的时候，只当一个 id
            // 用于区分不同的 Patch
            var harmony = new HarmonyLib.Harmony("ml.zhangjin");

            // 获得一个程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 给指定的程序集打 Patch
            harmony.PatchAll(assembly);

            // 或者，隐式的就是当前程序集
            harmony.PatchAll();


        }

        public static void Detail()
        {
            // 还可以进行更多的控制，例如对指定的方法执行 Patch
            // add null checks to the following lines, they are omitted for clarity
            var original = typeof(TheClass).GetMethod("TheMethod");
            var prefix = typeof(MyPatchClass1).GetMethod("SomeMethod");
            var postfix = typeof(MyPatchClass2).GetMethod("SomeMethod");


            // 如果有多个 id 类型的 Patch
            // 此处还可以指定优先级
            var harmonyPostfix = new HarmonyMethod(postfix)
            {
                priority = Priority.Low,
                before = new[] { "that.other.harmony.user" }
            };

            var harmony = new Harmony("ml.zhangjin");

            // 指定方法，执行更精细的 Patch
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            // 还可以单独指定，使用命名参数的方式
            // harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            // harmony.Patch(original, prefix: new HarmonyMethod(prefix));

            // 执行原始方法，查看效果
            var theClass = new TheClass();
            theClass.TheMethod();

        }

        public static void Debug()
        {
            // 开启 Debug
            // 会在桌面创建一个 harmony.log.txt 文件
            HarmonyLib.Harmony.DEBUG = true;

            // 记录字符串日志
            FileLog.Log("something");

            // 还可以使用 Buffer 模式
            FileLog.LogBuffered("A");
            FileLog.LogBuffered("B");

            // 需要记得 Flush
            FileLog.FlushBuffer();
        }


        public static void ViewPatch()
        {
            // 使用静态方法，查看所有程序集的 Patch
            var originalMethods = Harmony.GetAllPatchedMethods();

            var harmony = new Harmony("ml.zhangjin");
            // 使用实例方法，查看关联程序集的 Patch
            var myOriginalMethods = harmony.GetPatchedMethods();

            // 还可以根据 MethodBase 查看 Patch
            // get the MethodBase of the original
            var original = typeof(TheClass).GetMethod("TheMethod");

            // 获取指定方法上的所有 Patch
            // retrieve all patches
            var patches = Harmony.GetPatchInfo(original);
            if (patches == null) return; // not patched

            // id 集合
            // get a summary of all different Harmony ids involved
            FileLog.Log("all owners: " + patches.Owners);

            // 查看详细信息
            // get info about all Prefixes/Postfixes/Transpilers
            foreach (var patch in patches.Prefixes)
            {
                FileLog.Log("index: " + patch.index);
                FileLog.Log("owner: " + patch.owner);
                FileLog.Log("patch method: " + patch.PatchMethod);
                FileLog.Log("priority: " + patch.priority);
                FileLog.Log("before: " + patch.before);
                FileLog.Log("after: " + patch.after);
            }
        }

        public static void CheckPatch()
        {
            // 检测是否有指定 id 的 Patch
            if (Harmony.HasAnyPatches("their.harmony.id"))
            {
                // Do ....
            }


            // 还可以得到这个 Patch 试图
            var dict = Harmony.VersionInfo(out var myVersion);
            FileLog.Log("My version: " + myVersion);
            foreach (var entry in dict)
            {
                var id = entry.Key;
                var version = entry.Value;
                FileLog.Log("Mod " + id + " uses Harmony version " + version);
            }
        }

        public static void Unpatching()
        {
            // 只要执行过 Patch 后，原始方法就没有了
            // 新的 IL Code 将替换它

            // 所以，对于 Unpatching 而言
            // 就是在上一个 Patch 上，再做一次移除上一次添加了 IL 的 Patch


            // every patch on every method ever patched (including others patches):
            var harmony = new Harmony("ml.zhangjin");
            harmony.UnpatchAll();

            // only the patches that one specific Harmony instance did:
            harmony.UnpatchAll("ml.zhangjin");

            // 同样，也可以对方法进行细粒度的操作

            var original = typeof(TheClass).GetMethod("TheMethod");

            // 只移除 Prefix Patch
            // all prefixes on the original method:
            harmony.Unpatch(original, HarmonyPatchType.Prefix);

            // all prefixes from that other Harmony user on the original method:
            harmony.Unpatch(original, HarmonyPatchType.Prefix, "their.harmony.id");

            // 移除所有的 Patch
            // all patches from that other Harmony user:
            harmony.Unpatch(original, HarmonyPatchType.All, "their.harmony.id");

            // 移除指定的 Patch
            // removing a specific patch:
            var patch = typeof(TheClass).GetMethod("SomePrefix");
            harmony.Unpatch(original, patch);
        }

        private class TheClass
        {
            public void TheMethod()
            {
                Console.WriteLine("TheClass.TheMethod");
            }
        }

        // prefix 使用
        private class MyPatchClass1
        {
            // 需要为静态方法
            // 注意，此处的返回值 bool 可以表示，要不要执行原始方法
            public static bool SomeMethod()
            {
                Console.WriteLine("MyPatchClass1.SomeMethod");

                // false 不执行原始方法
                // return false;

                // true 执行原始方法
                return true;
            }
        }


        // postfix 使用
        private class MyPatchClass2
        {
            // 需要为静态方法
            public static void SomeMethod()
            {
                Console.WriteLine("MyPatchClass2.SomeMethod");
            }
        }
    }
}
