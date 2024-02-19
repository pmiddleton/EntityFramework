// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions
{
    /// <summary>
    /// todo
    /// </summary>
    public class WindowRowRangeExpression : Expression, IPrintableExpression
    {
        /// <summary>
        /// todo
        /// </summary>
        public enum RowRange
        {
            /// <summary>
            /// todo
            /// </summary>
            Row,

            /// <summary>
            /// todo
            /// </summary>
            Range
        };

        /// <summary>
        /// todo
        /// </summary>
        public SqlExpression? Preceding { get; init; }

        /// <summary>
        /// todo
        /// </summary>
        public SqlExpression? Following { get; init; }

        /// <summary>
        /// todo
        /// </summary>
        public RowRange RowOrRange { get; init; }
        
        /// <summary>
        /// todo
        /// </summary>
        public WindowRowRangeExpression(RowRange rowOrRange, SqlExpression? preceding, SqlExpression? following)
        {
            //todo - exception if both null?

            Preceding = preceding;
            Following = following;
            RowOrRange = rowOrRange;
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
         => this;

        /// <inheritdoc />
        void IPrintableExpression.Print(ExpressionPrinter expressionPrinter)
        {
            //todo
        }
    }
}
