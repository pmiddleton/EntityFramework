// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal
{
    /// <summary>
    /// todo
    /// </summary>
    public class PivotExpressionNode : ResultOperatorExpressionNodeBase, IQuerySourceExpressionNode
    {
        private readonly LambdaExpression _pivotSelector;
        private readonly LambdaExpression _aggregate;
        private readonly LambdaExpression _resultSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedPivotSelector;
        private readonly ResolvedExpressionCache<Expression> _cachedAggregate;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static MethodInfo[] SupportedMethods = typeof(PivotExtension).GetMethods(BindingFlags.Public | BindingFlags.Static) ;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public PivotExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression pivotSelector, LambdaExpression aggregate, LambdaExpression resultSelector)
            : base(parseInfo, null, null)
        {
            _pivotSelector = pivotSelector;
            _aggregate = aggregate;
            _resultSelector = resultSelector;

            _cachedPivotSelector = new ResolvedExpressionCache<Expression>(this);
            _cachedAggregate = new ResolvedExpressionCache<Expression>(this);
        }
      
        private Expression GetResolvedPivotSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedPivotSelector.GetOrCreate(r => r.GetResolvedExpression(_pivotSelector.Body, _pivotSelector.Parameters[0], clauseGenerationContext));
        }

        private Expression GetResolvedAggregateSelector(ClauseGenerationContext clauseGenerationContext)
        {
            return _cachedAggregate.GetOrCreate(r => r.GetResolvedExpression(_aggregate.Body, _aggregate.Parameters[0], clauseGenerationContext));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
            => QuerySourceExpressionNodeUtility.ReplaceParameterWithReference(
                this,
                inputParameter,
                expressionToBeResolved,
                clauseGenerationContext);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            var resolvedPivotSelector = GetResolvedPivotSelector(clauseGenerationContext);
            var resolvedAggregate = GetResolvedAggregateSelector(clauseGenerationContext);

            //var aggResolve = Source.Resolve(_aggregate.Parameters[0], _aggregate.Body, clauseGenerationContext);
            //var pivotResolve = Source.Resolve(_pivotSelector.Parameters[0], _pivotSelector.Body, clauseGenerationContext);
            var resultSelectorResolve = Source.Resolve(_resultSelector.Parameters[0], _resultSelector.Body, clauseGenerationContext);

            return new PivotResultOperator(AssociatedIdentifier, resolvedPivotSelector, resolvedAggregate, resultSelectorResolve);
        }
    }
}
