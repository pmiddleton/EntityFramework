// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides common language runtime (CLR) methods that expose database functions
    /// for use in <see cref="DbContext" /> LINQ to Entities queries.
    /// </summary>
    public static class SqlServerDbFunctionExtensions
    {
        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// matches the given expression against the given pattern.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database using the PatIndex method.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="pattern"> The pattern to match against </param>
        /// <param name="expression"> The expression to match. </param>
        /// <returns> Returns the starting position of the first occurrence of a pattern in a specified expression, or zero if the pattern is not found. </returns>
        public static int? PatIndex([NotNull] this DbFunctions func, [NotNull] string pattern, [CanBeNull] string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// matches the given expression against the given pattern using a like statement
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="pattern"> The pattern to match against </param>
        /// <param name="expression"> The expression to match. </param>
        /// <returns> Returns true if the expression matches the pattern, otherwise false. </returns>
        public static bool Like([NotNull] this DbFunctions func, [NotNull] string pattern, [CanBeNull] string expression)
        {
            throw new NotImplementedException();
        }
    }
}
