// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions.Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Query
{
    internal class WindowBuilderExpression : Expression
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        private readonly List<OrderingExpression> _orderingExpressions = new List<OrderingExpression>();
        private WindowPartitionExpression? _partitionExpression;
        private WindowFrameExpression? _frameExpression;

        public WindowBuilderExpression(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public IReadOnlyList<OrderingExpression> OrderingExpressions => _orderingExpressions;
        public WindowPartitionExpression? PartitionExpression => _partitionExpression;
        public WindowFrameExpression? FrameExpression => _frameExpression;

        public virtual void AddOrdering(SqlExpression expression, bool ascending) => _orderingExpressions.Add(new OrderingExpression(expression, ascending));
        public virtual void AddPartitionBy(SqlExpression[] partitions) => _partitionExpression = _sqlExpressionFactory.PartitionBy(partitions);
        public virtual void AddFrame(MethodInfo method, SqlExpression? preceding, SqlExpression? following) => _frameExpression = _sqlExpressionFactory.WindowFrame(method, preceding, following);
    }
}
