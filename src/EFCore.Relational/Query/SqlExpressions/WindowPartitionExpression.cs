// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions
{
    namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions
    {
        /// <summary>
        /// test
        /// </summary>
        public class WindowPartitionExpression : Expression, IPrintableExpression
        {
            /// <summary>
            /// todo
            /// </summary>
            public IReadOnlyList<SqlExpression> Partitions { get;  init; }

            /// <summary>
            /// tests
            /// </summary>
            /// <param name="partitions">test</param>
            public WindowPartitionExpression(IReadOnlyList<SqlExpression> partitions)
            {
                Partitions = partitions;
            }

            /// <summary>
            /// todo
            /// </summary>
            /// <param name="expressionPrinter">todo</param>
            public void Print(ExpressionPrinter expressionPrinter)
            {
                expressionPrinter.Append("PARTITION BY ");
            }

            /// <inheritdoc />
            protected override Expression VisitChildren(ExpressionVisitor visitor)
            {
                var newParts = new List<SqlExpression>();

                bool changed = false;

                foreach(var partition in Partitions)
                {
                    var newPart = (SqlExpression)visitor.Visit(partition);

                    newParts.Add(newPart);

                    changed |= partition != newPart;
                }

                return changed
                    ? new WindowPartitionExpression(newParts)
                    : this;
            }
        }
    }
}
