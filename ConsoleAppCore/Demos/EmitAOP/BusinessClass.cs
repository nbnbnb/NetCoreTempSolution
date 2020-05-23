using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    public class BusinessClass : IBusinessClass
    {
        public int NoArgument()
        {
            return 123;
        }

        [Action]
        public void Test()
        {

        }
    }
}
