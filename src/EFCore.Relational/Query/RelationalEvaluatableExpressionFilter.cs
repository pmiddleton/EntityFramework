// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     <para>
    ///         Represents a filter for evaluatable expressions.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
    ///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class RelationalEvaluatableExpressionFilter : EvaluatableExpressionFilter
    {
        /// <summary>
        ///     <para>
        ///         Creates a new <see cref="RelationalEvaluatableExpressionFilter" /> instance.
        ///     </para>
        ///     <para>
        ///         This type is typically used by database providers (and other extensions). It is generally
        ///         not used in application code.
        ///     </para>
        /// </summary>
        /// <param name="dependencies"> The dependencies to use. </param>
        /// <param name="relationalDependencies"> The relational-specific dependencies to use. </param>
        public RelationalEvaluatableExpressionFilter(
            [NotNull] EvaluatableExpressionFilterDependencies dependencies,
            [NotNull] RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
            : base(dependencies)
        {
            Check.NotNull(relationalDependencies, nameof(relationalDependencies));

            RelationalDependencies = relationalDependencies;
        }

        /// <summary>
        ///     Dependencies used to create a <see cref="RelationalEvaluatableExpressionFilter" />
        /// </summary>
        protected virtual RelationalEvaluatableExpressionFilterDependencies RelationalDependencies { get; }

        public override bool IsEvaluatableExpression(Expression expression, IModel model)
        {
            Check.NotNull(expression, nameof(expression));
            Check.NotNull(model, nameof(model));

            /*if (expression is MethodCallExpression methodCallExpression
                && model.FindDbFunction(methodCallExpression.Method) != null)
            {
                return false;
            }*/

            if (expression is MethodCallExpression methodCallExpression)
            {
                var dbFunction = model.FindDbFunction(methodCallExpression.Method);

                if (dbFunction != null)
                    return dbFunction.IsIQueryable;
            }

            return base.IsEvaluatableExpression(expression, model);
        }

        public override bool IsQueryableFunction(Expression expression, IModel model) => 
            expression is MethodCallExpression methodCallExpression
               && model.FindDbFunction(methodCallExpression.Method)?.IsIQueryable == true;
    }
}
