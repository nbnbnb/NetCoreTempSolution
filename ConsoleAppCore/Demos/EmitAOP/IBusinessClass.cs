using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    public interface IBusinessClass
    {
        void VoidAndVoid();

        int IntAndNoArgument();

        String StringAndString(String str);

        void VoidAndDate(DateTime dt);
    }
}
