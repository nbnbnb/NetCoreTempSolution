using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace ConsoleAppCore.MyLinq
{
    internal class DbExpressionVisitor : ExpressionVisitor
    {
        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Table:
                    return this.VisitTable((TableExpression)exp);
                case DbExpressionType.Column:
                    return this.VisitColumn((ColumnExpression)exp);
                case DbExpressionType.Select:
                    return this.VisitSelect((SelectExpression)exp);
                case DbExpressionType.Projection:
                    return this.VisitProjection((ProjectionExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitTable(TableExpression table)
        {
            return table;
        }

        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            return column;
        }

        protected virtual Expression VisitSelect(SelectExpression select)
        {
            Expression from = this.VisitSource(select.From);
            Expression where = this.Visit(select.Where);

            // 构建新的查询列
            ReadOnlyCollection<ColumnDeclaration> columns = this.VisitColumnDeclarations(select.Columns);

            // 如果任何一个条件改变
            // 则重新递归访问
            if (from != select.From || where != select.Where || columns != select.Columns)
            {
                return new SelectExpression(select.Type, select.Alias, columns, from, where);
            }

            return select;
        }

        /// <summary>
        /// 解析确切的表达式类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected virtual Expression VisitSource(Expression source)
        {
            return this.Visit(source);
        }

        /// <summary>
        /// 解析投射表达式
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        protected virtual Expression VisitProjection(ProjectionExpression proj)
        {
            SelectExpression source = (SelectExpression)this.Visit(proj.Source);

            Expression projector = this.Visit(proj.Projector);

            // 如果任何一个条件改变
            // 则重新递归访问
            if (source != proj.Source || projector != proj.Projector)
            {
                return new ProjectionExpression(source, projector);
            }
            return proj;
        }

        protected ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            List<ColumnDeclaration> alternate = null;
            for (int i = 0, n = columns.Count; i < n; i++)
            {
                ColumnDeclaration column = columns[i];

                // 递归解析 列表达式
                Expression e = this.Visit(column.Expression);

                if (alternate == null && e != column.Expression)
                {
                    alternate = columns.Take(i).ToList();
                }

                if (alternate != null)
                {
                    alternate.Add(new ColumnDeclaration(column.Name, e));
                }
            }

            if (alternate != null)
            {
                return alternate.AsReadOnly();
            }

            return columns;
        }
    }
}
