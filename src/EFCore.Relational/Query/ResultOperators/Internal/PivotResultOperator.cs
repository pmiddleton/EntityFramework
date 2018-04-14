// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class PivotResultOperator : SequenceFromSequenceResultOperatorBase, IQuerySource
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ItemName { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public Type ItemType { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public Expression PivotSelector { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public Expression Aggregate { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public Expression AggregateColumn { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public Expression ResultSelector { get; private set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PivotResultOperator(string itemName, Expression pivotSelector, Expression aggregate, Expression resultSelector)
        {
            PivotSelector = pivotSelector;
            ResultSelector = resultSelector;
            ItemName = itemName;
            ItemType = resultSelector.Type;

            //todo - I don't like any of this - there must be a better way?
            Aggregate = ((UnaryExpression)aggregate).Operand;
            var aggQueryModel = ((SubQueryExpression)Aggregate).QueryModel;

            var fromExp = aggQueryModel.MainFromClause.FromExpression;

            if (fromExp is NewExpression newExp)
            {
                AggregateColumn = newExp.Arguments.Single(a => ((MemberExpression)a).Member.Name == ((MemberExpression)aggQueryModel.SelectClause.Selector).Member.Name);
            }
            else
            {
                AggregateColumn = (MemberExpression)aggQueryModel.SelectClause.Selector;
            }

            //todo - figure out if we can cut down on this logic - it is needed if the main query is dbcontext
            if(AggregateColumn is MemberExpression memExp && memExp.Expression is QuerySourceReferenceExpression qsre && 
                qsre.ReferencedQuerySource is MainFromClause mfc && mfc.FromExpression is QuerySourceReferenceExpression mfcQsre)
            {
                AggregateColumn = Expression.Property(mfcQsre, memExp.Member.Name);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            return new StreamedSequenceInfo(typeof(IQueryable<>).MakeGenericType(ItemType), new QuerySourceReferenceExpression(this));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ResultOperatorBase Clone(CloneContext cloneContext) =>
            new PivotResultOperator(ItemName, PivotSelector, Aggregate, ResultSelector);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
            PivotSelector = transformation(PivotSelector);
            Aggregate = transformation(Aggregate);
            ResultSelector = transformation(ResultSelector);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override StreamedSequence ExecuteInMemory<T>(StreamedSequence input) => input;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override string ToString()
        {
            return "Pivot";
        }
    }
}
