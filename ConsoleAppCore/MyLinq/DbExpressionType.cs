using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal enum DbExpressionType
    {
        Table = 1000, // make sure these don't overlap with ExpressionType
        Column,
        Select,
        Projection
    }
}
