using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class TranslateResult
    {
        internal string CommandText;
        internal LambdaExpression Projector;
    }
}
