using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class ColumnProjection
    {
        // 表示需要查询的列字符串，类似于 A, B, C, D
        internal string Columns;

        // 一个表达式，通过 Expression.Lambda 方法转换后符合 Func<ProjectionRow, T> 的形式
        internal Expression Selector;
    }
}
