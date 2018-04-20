using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal enum DbExpressionType
    {
        Table = 1000,  // 设置一个较大的值，避免和 ExpressionType 冲突
        Column,
        Select,
        Projection
    }
}
