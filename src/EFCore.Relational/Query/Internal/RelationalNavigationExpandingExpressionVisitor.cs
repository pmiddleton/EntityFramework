// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class RelationalNavigationExpandingExpressionVisitor : NavigationExpandingExpressionVisitor
    {
        public RelationalNavigationExpandingExpressionVisitor(
            [NotNull] QueryCompilationContext queryCompilationContext,
            [NotNull] IEvaluatableExpressionFilter evaluatableExpressionFilter)
            : base(queryCompilationContext, evaluatableExpressionFilter)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            var dbFunction = QueryCompilationContext.Model.FindDbFunction(methodCallExpression.Method);

            return dbFunction?.IsIQueryable == true
                ? CreateNavigationExpansionExpression(methodCallExpression, QueryCompilationContext.Model.FindEntityType(dbFunction.MethodInfo.ReturnType.GetGenericArguments()[0]))
                : base.VisitMethodCall(methodCallExpression);
        }
        

        /*protected override bool IsValidMethodQuerySource(MethodInfo method, out IEntityType entityType)
        {
            var dbFunction = QueryCompilationContext.Model.FindDbFunction(method);

            if(dbFunction?.IsIQueryable == true)
            {
                entityType = QueryCompilationContext.Model.FindEntityType(dbFunction.MethodInfo.ReturnType.GetGenericArguments()[0]);
                return true;
            }

            entityType = null;
            return false;
        }*/
    }
}
