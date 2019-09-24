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
            // 查看解析日志
            // customVisitor.VisitQueryModel(queryModel);

            // Create an expression that returns the current item when invoked.
            // 创建一个读取属性的表达式树
            Expression currentItemExpression = Expression.Property(Expression.Constant(this), "Current");

            //  Now replace references like the "i" in "select i" that refers to the "i" in "from i in items"
            var mapping = new QuerySourceMapping();
            // 创建一个映射
            // 
            mapping.AddMapping(queryModel.MainFromClause, currentItemExpression);

            // 通过 Mapping
            // 将表达式参数进行替换
            // queryModel.MainFromClause 是 {from SampleDataSourceItem i in value(ConsoleAppCore.Demos.ReLinq.SampleQueryable`1[ConsoleAppCore.Demos.ReLinq.SampleDataSourceItem])}
            // 将其替换成 Current 属性
            // ReferenceReplacingExpressionVisitor.ReplaceClauseReferences 方法，根据 Mapping
            // 将会替换所有的 Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression 对象
            // 此处
            // 将 {[i]} 替换为 {value(ConsoleAppCore.Demos.ReLinq.SampleQueryExecutor).Current}
            queryModel.TransformExpressions(m => ReferenceReplacingExpressionVisitor.ReplaceClauseReferences(m, mapping, true));

            //SelectClause	{select new <>f__AnonymousType0`2(KKKing = [i].Name, JJJ = [i].Description)}	Remotion.Linq.Clauses.SelectClause
            //Selector	{new <>f__AnonymousType0`2(KKKing = [i].Name, JJJ = [i].Description)}	System.Linq.Expressions.Expression {System.Linq.Expressions.NewExpression}
            // 转换后
            //SelectClause	{select new <>f__AnonymousType0`2(KKKing = value(ConsoleAppCore.Demos.ReLinq.SampleQueryExecutor).Current.Name, JJJ = value(ConsoleAppCore.Demos.ReLinq.SampleQueryExecutor).Current.Description)}	Remotion.Linq.Clauses.SelectClause
            //Selector	{new <>f__AnonymousType0`2(KKKing = value(ConsoleAppCore.Demos.ReLinq.SampleQueryExecutor).Current.Name, JJJ = value(ConsoleAppCore.Demos.ReLinq.SampleQueryExecutor).Current.Description)}	System.Linq.Expressions.Expression {System.Linq.Expressions.NewExpression}

            // Create a lambda that takes our SampleDataSourceItem and passes it through the select clause
            // to produce a type of T.  (T may be SampleDataSourceItem, in which case this is an identity function.)
            // 构造 Lambda 中要用的参数
            var currentItemProperty = Expression.Parameter(typeof(SampleDataSourceItem));
            // 最终的 Lambda
            // 输入一个 SampleDataSourceItem，最后投影成 T
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

                Console.WriteLine($"****** yield {i}******");

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
