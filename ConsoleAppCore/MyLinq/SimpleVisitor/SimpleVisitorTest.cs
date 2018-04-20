using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.SimpleVisitor
{
    class SimpleVisitorTest
    {
        public static void Run()
        {
            Expression<Func<int, int, int>> addition = (a, b) => a + b;
            Visitor.CreateFromExpression(addition).Visit("");
        }
    }
}
