﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Math;  // 1. 静态类型导入

namespace ConsoleAppCore.Demos
{
    /// <summary>
    /// C# 6.0 新特性
    /// C# 6.0 released with .NET 4.6 and VS2015 (July 2015).
    /// </summary>
    public class CSharp60Features
    {
        public static void DoIt()
        {
            Console.WriteLine(Cos(1.23));  // 使用 Math 类方法，通过静态类型导入实现，不需要添加类名
        }

        /// <summary>
        /// 2. 异常过滤器
        /// 基本 when 过滤
        /// </summary>
        /// <returns></returns>
        public static async Task<string> MakeRequestWithNotModifiedSupport()
        {
            var client = new System.Net.Http.HttpClient();
            var streamTask = client.GetStringAsync("https://localhost:10000");
            try
            {
                return await streamTask;
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("301"))
            {
                return "Site Moved";
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("304"))
            {
                return "Use the Cache";
            }
        }


        /// <summary>
        /// 2. 异常过滤器
        /// 用作日志记录（将全匹配放在最前面，并且永远返回 false）
        /// </summary>
        public static void MethodThatFailsButHasRecoveryPath()
        {
            try
            {
                // PerformFailingOperation();
            }
            catch (Exception e) when (e.LogException())  // when 条件中永远返回 false，此处 LogException() 方法永远返回 false
            {
                // This is never reached!
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.ToString());
                // This can still catch the more specific
                // exception because the exception filter
                // above always returns false.
                // Perform recovery here 
            }
        }

        /// <summary>
        /// 3. 异常过滤器
        /// 设置调试时不捕获指定异常
        /// </summary>
        public static void MethodThatFailsWhenDebuggerIsNotAttached()
        {
            try
            {
                //PerformFailingOperation();
            }
            catch (Exception e) when (e.LogException())
            {
                // This is never reached!
            }
            catch (ArgumentNullException ex) when (!System.Diagnostics.Debugger.IsAttached)  // 判断调试模式
            {
                Console.WriteLine(ex.ToString());
                // Only catch exceptions when a debugger is not attached.
                // Otherwise, this should stop in the debugger. 
            }
        }

        /// <summary>
        /// 4. Await 在 catch 和 finally 块中的使用
        /// 在 C# 5.0 中限制了 await 在 catch 和 finally 中的使用
        /// </summary>
        /// <returns></returns>
        public static async Task<string> MakeRequestAndLogFailures()
        {
            await Task.Delay(1000);

            var client = new System.Net.Http.HttpClient();
            var streamTask = client.GetStringAsync("https://localhost:10000");
            try
            {
                var responseText = await streamTask;
                return responseText;
            }
            catch (System.Net.Http.HttpRequestException e) when (e.Message.Contains("301"))
            {
                await Task.Delay(2000);
                return "Site Moved";
            }
            finally
            {
                await Task.Delay(3000);
                client.Dispose();
            }
        }

        /// <summary>
        /// 5. 只读的自动属性
        /// 和 readonly 字段一样，只能在初始化或构造函数中赋值
        /// 构造函数中的赋值将会覆盖初始化的赋值
        /// </summary>
        public int Age { get; } = 121;

        /// <summary>
        /// 6. 自动属性初始化器
        /// </summary>
        public string UserName { get; set; } = "ZhangJin";

        /// <summary>
        /// 7. 表达式体成员 
        /// 使用 => 操作符
        /// 适用于方法和只读属性
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"The Name is ${UserName}";  // 方法
        public string BigName => $"Big{UserName}";  // 属性（只读）

        /// <summary>
        /// 8. 空判断操作符
        /// </summary>
        public static void NullChecker()
        {
            // default 是 C# 7.1 中的新功能
            CSharp60Features cSharp60 = default;

            // 不为 null 时调用对象的实例方法
            // 与 Kotlin 中一致
            cSharp60?.InstanceMethod();

            // 不为 null 时读取属性
            String name = cSharp60?.UserName;

            // ?. 操作符可以保证左边的表达式只被计算一次
            // 并且将结果缓存，这样可以避免委托为空的场景
            // Delegate 是值类型，复制下来就可以保证不能空的场景了
            cSharp60.SometingHappened?.Invoke(null, null);
        }

        /// <summary>
        /// 9. 字符串的内联解释器
        /// 可以调用方法，使用 LINQ 等等
        /// 建议简单逻辑使用内联语法，复杂语句还是使用方法比较合适
        /// </summary>
        public void StringInterpolation()
        {
            Console.WriteLine($@"My Name Length is {UserName.Length}, {String.Join('|', UserName.ToCharArray())}");
        }

        /// <summary>
        /// 10. nameof 操作符
        /// 用于获取变量、属性和成员的名称
        /// 返回的是短限定名称（不包含命名空间信息）
        /// 
        /// 优点
        /// 在重构时，是无法对字符串名称进行探测的，使用 nameof 操作符，则不会有这种问题
        /// 对于 INotifyPropertyChanged 接口尤其有用
        /// </summary>
        public void NameofOperator()
        {
            Console.WriteLine(nameof(UserName));
        }

        /// <summary>
        /// 11. 字典初始化器
        /// </summary>
        public void DictInit()
        {
            // 方式一
            Dictionary<string, int> infos = new Dictionary<string, int>
            {
                ["users"] = 123,
                ["abc"] = 456
            };

            // 方式二
            // 感觉这种方式更简单
            infos = new Dictionary<string, int>
            {
                {"A",123 },
                {"B",456 }
            };
        }

        /// <summary>
        /// 12. 扩展集合的 Add 方法
        /// </summary>
        public void ExtCollectionAddMethod()
        {
            MySet mm = new MySet
            {
                // 在 C# 5.0 中，要求 MySet 中必须要有一个 void Add(CSharp60Features obj) 的实例方法
                // 在 C# 6.0 中，可以将这个方法定义为一个扩展方法进行实现
                // 可以添加多种不同的扩展方法，所以实例化的时候可以添加多种对象
                // 这种可扩展性，对于在集合中初始化其他类型，非常方便

                // 同时也解决了在不能修改源码的情况下执行，也能使用集合初始化语法
                new CSharp60Features(),
                new CSharp60Features()
            };
        }

        public void InstanceMethod() { }

        public event EventHandler<EventArgs> SometingHappened = null;
    }

    /// <summary>
    /// 自定义 List
    /// </summary>
    class MySet : List<MySet>
    {

    }

    static class Ext
    {
        /// <summary>
        /// 日志记录方法永远返回 false
        /// 这样在异常过滤器中可以继续向下匹配
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool LogException(this Exception e)
        {
            Console.Error.WriteLine(@"Exceptions happen: {e}");
            return false;
        }

        /// <summary>
        /// 这个 MySet 的一个扩展方法
        /// 只要符合此签名，能使用集合初始化语法
        /// </summary>
        /// <param name="e"></param>
        /// <param name="s"></param>
        public static void Add(this MySet e, CSharp60Features s)
        {

        }
    }
}
