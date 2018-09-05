using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.SimpleAOP
{
    public class TargetClass : ITargetInterface
    {
        public string GetAddress(int no)
        {
            return $"Address is No.{no}";
        }

        public void WriteMessage(string msg)
        {
            Console.WriteLine("Write Some Message");
        }
    }
}
