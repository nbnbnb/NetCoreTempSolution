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

            var results = from i in items
                              // where i.Description == "KKKing" && i.Name == "JJZhang"
                          select new { KKKing = i.Name, JJJ = i.Description };


            // force evalution of the statement to prevent assertion from re-evaluating the query.
            Console.WriteLine(" 一次性 yield");
            results.ToList().ForEach(it =>
            {
                Console.WriteLine(it);
            });


            Console.WriteLine("每次 yield");
            foreach (var it in results)
            {
                Console.WriteLine(it);
            }


        }
    }
}
