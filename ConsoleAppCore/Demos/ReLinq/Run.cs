using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleAppCore.Demos.ReLinq
{
    internal class Run
    {
        public static void ReLinq()
        {
            var queryParser = QueryParser.CreateDefault();

            // Create our IQueryable instance
            SampleQueryable<SampleDataSourceItem> items = new SampleQueryable<SampleDataSourceItem>(queryParser, new SampleQueryExecutor());

            var results = from i in items select i;

            // force evalution of the statement to prevent assertion from re-evaluating the query.
            var list = results.ToList();
        }
    }
}
