using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionVisitors;
using Remotion.Linq.Parsing.Structure;

namespace ConsoleAppCore.Demos.ReLinq
{
    internal class SampleQueryExecutor : IQueryExecutor
    {
        // Set up a proeprty that will hold the current item being enumerated.
        public SampleDataSourceItem Current { get; private set; }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            CustomVisitor customVisitor = new CustomVisitor();
            customVisitor.VisitQueryModel(queryModel);

            // Create an expression that returns the current item when invoked.
            Expression currentItemExpression = Expression.Property(Expression.Constant(this), "Current");

            //  Now replace references like the "i" in "select i" that refers to the "i" in "from i in items"
            var mapping = new QuerySourceMapping();
            mapping.AddMapping(queryModel.MainFromClause, currentItemExpression);
            queryModel.TransformExpressions(m => ReferenceReplacingExpressionVisitor.ReplaceClauseReferences(m, mapping, true));

            // Create a lambda that takes our SampleDataSourceItem and passes it through the select clause
            // to produce a type of T.  (T may be SampleDataSourceItem, in which case this is an identity function.)
            var currentItemProperty = Expression.Parameter(typeof(SampleDataSourceItem));
            var projection = Expression.Lambda<Func<SampleDataSourceItem, T>>(queryModel.SelectClause.Selector, currentItemProperty);
            var projector = projection.Compile();

            // Pretend we're getting SampleDataSourceItems from somewhere...
            for (var i = 0; i < 10; i++)
            {
                Current = new SampleDataSourceItem
                {
                    Name = "Name " + i,
                    Description = "This describes the item in position " + i
                };

                // Use the projector to convert (if necessary) the current item to what is being selected and return it.
                yield return projector(Current);
            }
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            // We'll get to this one later...
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);
            return returnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
        }
    }
}
