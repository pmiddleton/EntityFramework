﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class NavigationExpandingExpressionVisitorFactory : INavigationExpandingExpressionVisitorFactory
    {
        public NavigationExpandingExpressionVisitor Create(
            QueryCompilationContext queryCompilationContext, IEvaluatableExpressionFilter evaluatableExpressionFilter)
        {
            return new NavigationExpandingExpressionVisitor(queryCompilationContext, evaluatableExpressionFilter);
        }
    }
}
