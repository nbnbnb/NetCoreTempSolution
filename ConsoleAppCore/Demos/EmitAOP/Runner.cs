using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    public static class Runner
    {
        public static void Run()
        {
            IBusinessClass business = CustomDynamicProxy.CreateProxyOfRealize<IBusinessClass, BusinessClass>();
            business.VoidAndVoid();
            Console.WriteLine("---------------------------------------");
            business.VoidAndDate(DateTime.Now);
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(business.IntAndNoArgs());
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(business.StringAndString("haha"));
            Console.WriteLine("---------------------------------------");
            business.VoidAndNoArgs();
            Console.WriteLine("---------------------------------------");
            Console.WriteLine(business.StringAndNoArgs());
        }
    }
}
