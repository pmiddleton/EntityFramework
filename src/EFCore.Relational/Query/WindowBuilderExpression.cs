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
        private WindowRowRangeExpression? _rowRangeExpression;

        public WindowBuilderExpression(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public IReadOnlyList<OrderingExpression> OrderingExpressions => _orderingExpressions;
        public WindowPartitionExpression? PartitionExpression => _partitionExpression;
        public WindowRowRangeExpression? RowRangeExpression => _rowRangeExpression;

        public void AddOrdering(SqlExpression expression, bool ascending) => _orderingExpressions.Add(new OrderingExpression(expression, ascending));
        public void AddPartitionBy(SqlExpression[] partitions) => _partitionExpression = _sqlExpressionFactory.PartitionBy(partitions);

        public void AddRowOrRange(WindowRowRangeExpression.RowRange rowOrRange, SqlExpression? preceding, SqlExpression? following)
            => _rowRangeExpression = new WindowRowRangeExpression(rowOrRange, preceding, following);
    }
}
