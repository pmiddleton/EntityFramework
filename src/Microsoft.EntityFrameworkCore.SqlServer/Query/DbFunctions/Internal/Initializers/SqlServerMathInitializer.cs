// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.DbFunctions.Internal.Initializers
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqlServerMathInitializer : IDbFunctionInitializer
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Initialize(ModelBuilder modelBuilder)
        {
            /* Round */
            var roundMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Round)).Where(m => (m.GetParameters().Length == 1)
                        || ((m.GetParameters().Length == 2) && (m.GetParameters()[1].ParameterType == typeof(int))));
            modelBuilder.DbFunction(roundMI, b =>
            {
                //Math.Round(
                b.HasSchema("").HasName("ROUND");

                if (b.Metadata.MethodInfo.GetParameters().Length == 1)
                    b.Parameter("length").HasParameterIndex(1).HasValue(0);
            });

            /* Truncate */
            var truncateMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Truncate));
            modelBuilder.DbFunction(truncateMI, b =>
            {
                b.HasSchema("").HasName("ROUND");

                b.Parameter("length").HasParameterIndex(1).HasValue(0);
                b.Parameter("function").HasParameterIndex(2).HasValue(1);
            });

            /* POW */
            var powMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Pow)).Single();
            modelBuilder.DbFunction(powMI, b =>
            {
                b.HasSchema("").HasName("POWER");
            });

            /* Floor */
            var floorMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Floor));
            modelBuilder.DbFunction(floorMI, b =>
            {
                b.HasSchema("").HasName("FLOOR");
            });

            /* Ceiling */
            var ceilingMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Ceiling));
            modelBuilder.DbFunction(ceilingMI, b =>
            {
                b.HasSchema("").HasName("CEILING");
            });

            /* ABS */
            var absgMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Abs));
            modelBuilder.DbFunction(absgMI, b =>
            {
                b.HasSchema("").HasName("ABS");
            });
        }
    }
}
