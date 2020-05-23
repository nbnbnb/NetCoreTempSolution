using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InterceptorAttribute : InterceptorBaseAttribute
    {
        public override object Invoke(object @object, string method, object[] parameters)
        {
            Console.WriteLine($"interceptor does something before invoke [{method}]");

            object obj = null;
            try
            {
                obj = base.Invoke(@object, method, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"interceptor does something after invoke [{method}]");

            return obj;
        }
    }
}
