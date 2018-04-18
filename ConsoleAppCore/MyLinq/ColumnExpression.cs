using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class ColumnExpression : Expression
    {
        string alias;
        string name;
        int ordinal;
        internal ColumnExpression(Type type, string alias, string name, int ordinal)
            : base((ExpressionType)DbExpressionType.Column, type)
        {
            this.alias = alias;
            this.name = name;
            this.ordinal = ordinal;
        }

        internal string Alias
        {
            get { return this.alias; }
        }

        internal string Name
        {
            get { return this.name; }
        }

        internal int Ordinal
        {
            get { return this.ordinal; }
        }
    }
}
