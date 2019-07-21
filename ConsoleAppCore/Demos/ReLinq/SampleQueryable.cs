using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleAppCore.Demos.ReLinq
{
    internal class SampleQueryable<T> : QueryableBase<T>
    {
        public SampleQueryable(IQueryParser queryParser, IQueryExecutor queryExecutor)
            : base(new DefaultQueryProvider(typeof(SampleQueryable<>), queryParser, queryExecutor))
        {

        }

        public SampleQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)

        {

        }
    }
}
