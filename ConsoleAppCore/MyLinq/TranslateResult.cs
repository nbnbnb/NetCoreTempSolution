using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class TranslateResult
    {
        // SQL 命令字符串
        internal string CommandText;

        // Select 投射类型
        // 为 Func<ProjectionRow, T> 的形式
        internal LambdaExpression Projector;
    }
}
