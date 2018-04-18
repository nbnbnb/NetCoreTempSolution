using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class ColumnDeclaration
    {
        string name;
        Expression expression;

        internal ColumnDeclaration(string name, Expression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        internal string Name
        {
            get { return this.name; }
        }

        internal Expression Expression
        {
            get { return this.expression; }
        }
    }
}
