// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class QueryTranslationPreprocessor
    {
        private readonly QueryCompilationContext _queryCompilationContext;
        private readonly INavigationExpandingExpressionVisitorFactory _navigationExpandingExpressionVisitorFactory;

        public QueryTranslationPreprocessor(
            [NotNull] QueryTranslationPreprocessorDependencies dependencies,
            [NotNull] QueryCompilationContext queryCompilationContext,
            [NotNull] INavigationExpandingExpressionVisitorFactory navigationExpandingExpressionVisitorFactory)
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(queryCompilationContext, nameof(queryCompilationContext));

            Dependencies = dependencies;
            _queryCompilationContext = queryCompilationContext;
            _navigationExpandingExpressionVisitorFactory = navigationExpandingExpressionVisitorFactory;
        }

        protected virtual QueryTranslationPreprocessorDependencies Dependencies { get; }

        public virtual Expression Process([NotNull] Expression query)
        {
            Check.NotNull(query, nameof(query));

            query = new EnumerableToQueryableMethodConvertingExpressionVisitor().Visit(query);
            query = new QueryMetadataExtractingExpressionVisitor(_queryCompilationContext).Visit(query);
            query = new InvocationExpressionRemovingExpressionVisitor().Visit(query);
            query = new AllAnyToContainsRewritingExpressionVisitor().Visit(query);
            query = new GroupJoinFlatteningExpressionVisitor().Visit(query);
            query = new NullCheckRemovingExpressionVisitor().Visit(query);
            query = new EntityEqualityRewritingExpressionVisitor(_queryCompilationContext).Rewrite(query);
            query = new SubqueryMemberPushdownExpressionVisitor().Visit(query);
            query = _navigationExpandingExpressionVisitorFactory.Create(_queryCompilationContext, Dependencies.EvaluatableExpressionFilter)
                .Expand(query);
            query = new FunctionPreprocessingExpressionVisitor().Visit(query);

            return query;
        }
    }
}
