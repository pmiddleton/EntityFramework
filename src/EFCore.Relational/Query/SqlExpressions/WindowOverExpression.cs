// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
    public IReadOnlyList<OrderingExpression> OrderingExpressions { get; init; }

    /// <summary>
    /// todo
    /// </summary>
    public WindowFrameExpression? WindowFrameExpression { get; init; }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="aggregateExpression">todo</param>
    /// <param name="partitionExpression">todo</param>
    /// <param name="orderingExpressions">todo</param>
    /// <param name="windowframeExpression">todo</param>
    public WindowOverExpression(SqlFunctionExpression aggregateExpression, WindowPartitionExpression? partitionExpression,
        IReadOnlyList<OrderingExpression> orderingExpressions, WindowFrameExpression? windowframeExpression)
        : base(aggregateExpression.Type, aggregateExpression.TypeMapping)
    {
        PartitionExpression = partitionExpression;
        AggregateExpression = aggregateExpression;
        OrderingExpressions = orderingExpressions;
        WindowFrameExpression = windowframeExpression;
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="aggregateExpression">todo</param>
    /// <param name="partitionExpression">todo</param>
    /// <param name="orderingExpressions">todo</param>
    /// <param name="windowframeExpression">todo</param>
    /// <param name="type">todo</param>
    /// <param name="relationalTypeMapping">todo</param>
    public WindowOverExpression(SqlFunctionExpression aggregateExpression, WindowPartitionExpression? partitionExpression,
        IReadOnlyList<OrderingExpression> orderingExpressions, WindowFrameExpression? windowframeExpression,
        Type type, RelationalTypeMapping? relationalTypeMapping)
        : base(type, relationalTypeMapping)
    {
        PartitionExpression = partitionExpression;
        AggregateExpression = aggregateExpression;
        OrderingExpressions = orderingExpressions;
        WindowFrameExpression = windowframeExpression;
    }

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var aggregate = (SqlFunctionExpression)visitor.Visit(AggregateExpression);
        var partition = PartitionExpression != null ? visitor.Visit(PartitionExpression) as WindowPartitionExpression : null;
        var orderBys = new List<OrderingExpression>();
        var rowRange = visitor.Visit(WindowFrameExpression) as WindowFrameExpression;

        var changed = false;

        foreach (var orderingExpression in OrderingExpressions)
        {
            var newOrder = (OrderingExpression)visitor.Visit(orderingExpression);
            orderBys.Add(newOrder);
            changed |= newOrder != orderingExpression;
        }

        return partition != PartitionExpression || aggregate != AggregateExpression || rowRange != WindowFrameExpression || changed
            ? new WindowOverExpression(aggregate, partition, orderBys, rowRange)
            : this;
    }


    /// <summary>
    ///     Applies supplied type mapping to this expression.
    /// </summary>
    /// <param name="typeMapping">A relational type mapping to apply.</param>
    /// <returns>A new expression which has supplied type mapping.</returns>
    public virtual WindowOverExpression ApplyTypeMapping(RelationalTypeMapping? typeMapping)
        => new(
            AggregateExpression.ApplyTypeMapping(typeMapping),
            PartitionExpression,
            OrderingExpressions,
            WindowFrameExpression,
            Type,
            typeMapping ?? TypeMapping);

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="expressionPrinter">todo</param>
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Append("OVER ");
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj != null
            && (ReferenceEquals(this, obj)
                || obj is WindowOverExpression windowOverExpression
                && Equals(windowOverExpression));

    private bool Equals(WindowOverExpression windowOverExpression)
        => base.Equals(windowOverExpression)
            && AggregateExpression.Equals(windowOverExpression.AggregateExpression)
            && ((PartitionExpression == null && windowOverExpression.PartitionExpression == null)
                || (PartitionExpression != null && PartitionExpression.Equals(windowOverExpression.PartitionExpression)))
            && ((WindowFrameExpression == null && windowOverExpression.WindowFrameExpression == null)
                || (WindowFrameExpression != null && WindowFrameExpression.Equals(windowOverExpression.WindowFrameExpression)))
            && ((OrderingExpressions == null && windowOverExpression.OrderingExpressions == null)
                || (OrderingExpressions != null && windowOverExpression.OrderingExpressions != null
                        && OrderingExpressions.SequenceEqual(windowOverExpression.OrderingExpressions)));

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(base.GetHashCode());
        hash.Add(AggregateExpression);
        hash.Add(WindowFrameExpression);
        hash.Add(WindowFrameExpression);

        if (OrderingExpressions != null)
        {
            for (var i = 0; i < OrderingExpressions.Count; i++)
            {
                hash.Add(OrderingExpressions[i]);
            }
        }

        return hash.ToHashCode();
    }
}
