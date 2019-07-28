using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ConsoleAppCore.Demos.ReLinq
{
    class CustomVisitor : QueryModelVisitorBase
    {
        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            Console.WriteLine("VisitWhereClause  --- Start");
            base.VisitWhereClause(whereClause, queryModel, index);
            Console.WriteLine("VisitWhereClause --- End");
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            Console.WriteLine("VisitSelectClause --- Start");
            base.VisitSelectClause(selectClause, queryModel);
            Console.WriteLine("VisitSelectClause --- End");
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            Console.WriteLine("VisitMainFromClause --- Start");
            base.VisitMainFromClause(fromClause, queryModel);
            Console.WriteLine("VisitMainFromClause --- End");
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            Console.WriteLine("VisitAdditionalFromClause --- Start");
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
            Console.WriteLine("VisitAdditionalFromClause --- End");
        }

        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
        {
            Console.WriteLine("VisitBodyClauses --- Start");
            base.VisitBodyClauses(bodyClauses, queryModel);
            Console.WriteLine("VisitBodyClauses --- End");
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            Console.WriteLine("VisitGroupJoinClause --- Start");
            base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
            Console.WriteLine("VisitGroupJoinClause --- End");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            Console.WriteLine("VisitJoinClause --- Start");
            base.VisitJoinClause(joinClause, queryModel, groupJoinClause);
            Console.WriteLine("VisitJoinClause --- End");
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            Console.WriteLine("VisitJoinClause --- Start");
            base.VisitJoinClause(joinClause, queryModel, index);
            Console.WriteLine("VisitJoinClause --- End");
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            Console.WriteLine("VisitOrderByClause --- Start");
            base.VisitOrderByClause(orderByClause, queryModel, index);
            Console.WriteLine("VisitOrderByClause --- End");
        }

        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
        {
            Console.WriteLine("VisitOrdering --- Start");
            base.VisitOrdering(ordering, queryModel, orderByClause, index);
            Console.WriteLine("VisitOrdering --- End");
        }

        protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
        {
            Console.WriteLine("VisitOrderings --- Start");
            base.VisitOrderings(orderings, queryModel, orderByClause);
            Console.WriteLine("VisitOrderings --- End");
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            Console.WriteLine("VisitResultOperator --- Start");
            base.VisitResultOperator(resultOperator, queryModel, index);
            Console.WriteLine("VisitResultOperator --- End");
        }

        protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
        {
            Console.WriteLine("VisitResultOperators --- Start");
            base.VisitResultOperators(resultOperators, queryModel);
            Console.WriteLine("VisitResultOperators --- End");
        }

    }
}
