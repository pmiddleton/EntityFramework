// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class RelationalNavigationExpandingExpressionVisitor : NavigationExpandingExpressionVisitor
    {
        public RelationalNavigationExpandingExpressionVisitor(
            QueryCompilationContext queryCompilationContext,
            IEvaluatableExpressionFilter evaluatableExpressionFilter)
            : base(queryCompilationContext, evaluatableExpressionFilter)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            var dbFunction = QueryCompilationContext.Model.FindDbFunction(methodCallExpression.Method);

            if (dbFunction?.IsIQueryable == true)
            {
                return CreateNavigationExpansionExpression(methodCallExpression, QueryCompilationContext.Model.FindEntityType(dbFunction.MethodInfo.ReturnType.GetGenericArguments()[0]));
            }

            return base.VisitMethodCall(methodCallExpression);
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
