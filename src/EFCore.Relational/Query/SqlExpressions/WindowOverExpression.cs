// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions.Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions
{
    /// <summary>
    /// test
    /// </summary>
    public class WindowOverExpression : SqlExpression, IPrintableExpression
    {
        /// <summary>
        /// todo
        /// </summary>
        public WindowPartitionExpression? PartitionExpression { get; init; }

        /// <summary>
        /// todo
        /// </summary>
        public SqlFunctionExpression AggregateExpression { get; init; }

        /// <summary>
        /// todo
        /// </summary>
        public List<OrderingExpression> OrderingExpressions { get; init; }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="aggregateExpression">todo</param>
        /// <param name="partitionExpression">todo</param>
        /// <param name="orderingExpressions">todo</param>
        public WindowOverExpression(SqlFunctionExpression aggregateExpression, WindowPartitionExpression? partitionExpression, List<OrderingExpression> orderingExpressions)
            : base(aggregateExpression.Type, aggregateExpression.TypeMapping)
        {
            PartitionExpression = partitionExpression;
            AggregateExpression = aggregateExpression;
            OrderingExpressions = orderingExpressions;
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var aggregate = (SqlFunctionExpression)visitor.Visit(AggregateExpression);
            var partition = PartitionExpression != null ? (WindowPartitionExpression?)visitor.Visit(PartitionExpression) : null;
            var orderBys = new List<OrderingExpression>();

            var changed = false;

            foreach (var orderingExpression in OrderingExpressions)
            {
                var newOrder = (OrderingExpression)visitor.Visit(orderingExpression);
                orderBys.Add(newOrder);
                changed |= newOrder != orderingExpression;
            }

            return partition != PartitionExpression || aggregate != AggregateExpression || changed
                ? new WindowOverExpression(aggregate, partition, orderBys)
                : this;
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="expressionPrinter">todo</param>
        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("OVER ");
        }
    }
}
