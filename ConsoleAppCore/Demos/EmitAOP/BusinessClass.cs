using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    [Interceptor]
    public class BusinessClass : IBusinessClass
    {
        [Action]
        public int IntAndNoArgs()
        {
            Console.WriteLine("IntAndNoArgs");
            return 123;
        }

        [Action]
        public string StringAndString(string str)
        {
            Console.WriteLine("StringAndString");
            return "---" + str + "---";
        }

        [Action]
        public void VoidAndDate(DateTime dt)
        {
            Console.WriteLine("VoidAndDate");
            Console.WriteLine($"Current DateTime {dt}");
        }

        [Action]
        public void VoidAndVoid()
        {
            Console.WriteLine("VoidAndVoid");
        }

        [Action]
        public void VoidAndNoArgs()
        {
            Console.WriteLine("VoidAndNoArgs");
        }

   
        public string StringAndNoArgs()
        {
            Console.WriteLine("StringAndNoArgs");
            return "Good Day!";
        }
    }
}
