using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.SimpleVisitor
{
    class ConstantVisitor : Visitor
    {
        private readonly ConstantExpression node;

        public ConstantVisitor(ConstantExpression node) : base(node)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            Console.WriteLine($"{prefix}This is an {NodeType} expression type");
            Console.WriteLine($"{prefix}The type of the constant value is {node.Type}");
            Console.WriteLine($"{prefix}The value of the constant value is {node.Value}");
        }
    }
}
