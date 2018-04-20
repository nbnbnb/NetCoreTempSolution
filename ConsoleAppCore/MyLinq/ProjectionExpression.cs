using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    internal class ProjectionExpression : Expression
    {
        SelectExpression source;
        Expression projector;

        internal ProjectionExpression(SelectExpression source, Expression projector)
            : base((ExpressionType)DbExpressionType.Projection, projector.Type)
        {
            this.source = source;
            this.projector = projector;
        }

        internal SelectExpression Source
        {
            get
            {
                return this.source;
            }
        }

        internal Expression Projector
        {
            get
            {
                return this.projector;
            }
        }
    }
}
