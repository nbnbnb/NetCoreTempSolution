using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    public static class Runner
    {
        public static void Run()
        {
            IBusinessClass business = DynamicProxy.CreateProxyOfRealize<IBusinessClass, BusinessClass>();
            business.VoidAndVoid();
            Console.WriteLine("---------------------------------------");
            business.VoidAndDate(DateTime.Now);
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(business.IntAndNoArgument());
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(business.StringAndString("haha"));
        }
    }
}
