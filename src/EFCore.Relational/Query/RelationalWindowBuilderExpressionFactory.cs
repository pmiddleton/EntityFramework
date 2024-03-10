// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Query;

/// <summary>
/// todo
/// </summary>
public class RelationalWindowBuilderExpression : Expression
{
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    private readonly List<OrderingExpression> _orderingExpressions = new List<OrderingExpression>();
    private WindowPartitionExpression? _partitionExpression;
    private WindowFrameExpression? _frameExpression;

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="sqlExpressionFactory">todo</param>
    public RelationalWindowBuilderExpression(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <summary>
    /// todo
    /// </summary>
    public IReadOnlyList<OrderingExpression> OrderingExpressions => _orderingExpressions;

    /// <summary>
    /// todo
    /// </summary>
    public WindowPartitionExpression? PartitionExpression => _partitionExpression;

    /// <summary>
    /// todo
    /// </summary>
    public WindowFrameExpression? FrameExpression => _frameExpression;

    /// <summary>
    /// todo
    /// </summary>
    public virtual void AddOrdering(SqlExpression expression, bool ascending) => _orderingExpressions.Add(new OrderingExpression(expression, ascending));

    /// <summary>
    /// todo
    /// </summary>
    public virtual void AddPartitionBy(SqlExpression[] partitions) => _partitionExpression = _sqlExpressionFactory.PartitionBy(partitions);

    /// <summary>
    /// todo
    /// </summary>
    public virtual void AddFrame(MethodInfo method, SqlExpression? preceding, SqlExpression? following) => _frameExpression = _sqlExpressionFactory.WindowFrame(method, preceding, following);
}
