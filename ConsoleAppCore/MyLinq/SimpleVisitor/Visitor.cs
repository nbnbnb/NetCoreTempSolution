using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq.SimpleVisitor
{
    public abstract class Visitor
    {
        private readonly Expression node;

        protected Visitor(Expression node)
        {
            this.node = node;
        }

        public abstract void Visit(string prefix);

        public ExpressionType NodeType => this.node.NodeType;

        public static Visitor CreateFromExpression(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    return new ConstantVisitor((ConstantExpression)node);
                case ExpressionType.Lambda:
                    return new LambdaVisitor((LambdaExpression)node);
                case ExpressionType.Parameter:
                    return new ParameterVisitor((ParameterExpression)node);
                case ExpressionType.Add:
                    return new BinaryVisitor((BinaryExpression)node);
                default:
                    Console.Error.WriteLine($"Node not processed yet: {node.NodeType}");
                    return default;
            }
        }
    }
}
