using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.EmitAOP
{
    public interface IBusinessClass
    {
        void VoidAndVoid();

        int IntAndNoArgs();

        string StringAndString(string str);

        void VoidAndDate(DateTime dt);

        void VoidAndNoArgs();

        string StringAndNoArgs();
    }
}
