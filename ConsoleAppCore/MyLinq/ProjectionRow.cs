using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    public abstract class ProjectionRow
    {
        public abstract object GetValue(int index);
    }
}
