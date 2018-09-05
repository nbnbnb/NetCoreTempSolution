using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.Demos.SimpleAOP
{
    public interface ITargetInterface
    {
        void WriteMessage(string msg);

        string GetAddress(int no);
    }
}
