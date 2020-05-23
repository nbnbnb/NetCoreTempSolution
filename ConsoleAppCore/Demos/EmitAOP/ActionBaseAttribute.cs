using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    /// <summary>
    /// Before 和 After 切面
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ActionBaseAttribute : Attribute
    {
        public virtual void Before(string @method, object[] parameters)
        {
            Console.WriteLine($"Action Before, method:{method},parameters:{parameters}");
        }

        public virtual object After(string @method, object result)
        {
            Console.WriteLine($"Action After,method:{method},result:{result}");
            return result;
        }
    }
}
