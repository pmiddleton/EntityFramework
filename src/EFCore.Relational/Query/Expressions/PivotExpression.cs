// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    ///     Represents a SQL PIVOT expression.
    /// </summary>
    public class PivotExpression : TableExpressionBase
    {
        /// <summary>
        /// todo
        /// </summary>
        public virtual Expression PivotColumn { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        public virtual SqlFunctionExpression Aggregate { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        public virtual Type ResultType { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        public virtual ColumnExpression[] ResultColumns { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="model">todo</param>
        /// <param name="pivotColumn">todo</param>
        /// <param name="aggregate">todo</param>
        /// <param name="resultType">todo</param>
        /// <param name="alias">todo</param>
        /// <param name="querySource">todo</param>
        public PivotExpression([NotNull] IModel model, [NotNull] Expression pivotColumn, [NotNull] SqlFunctionExpression aggregate,
            [NotNull] Type resultType, [CanBeNull] string alias, [NotNull] IQuerySource querySource)
            : base(querySource, alias)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(pivotColumn, nameof(pivotColumn));
            Check.NotNull(aggregate, nameof(aggregate));
            Check.NotNull(resultType, nameof(resultType));

            PivotColumn = pivotColumn;
            Aggregate = aggregate;
            ResultType = resultType;

            var entityType = model.FindEntityType(ResultType);

            if (entityType == null)
            {
                throw new Exception("unknown entity or query type");
            }

            ResultColumns = entityType.GetProperties().Where(p => p.Relational().IsPivot).Select(p => new ColumnExpression(p.Relational().ColumnName, p, this)).ToArray();
        }

        /// <summary>
        ///     Dispatches to the specific visit method for this node type.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is ISqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitPivotExpression(this)
                : base.Accept(visitor);
        }
    }
}
