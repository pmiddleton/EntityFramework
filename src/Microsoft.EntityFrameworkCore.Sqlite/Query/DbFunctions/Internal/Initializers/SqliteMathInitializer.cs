// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query.DbFunctions.Internal.Initializers
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqliteMathInitializer : IDbFunctionInitializer
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

            /* Min */
            var mathMinMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Min));
            modelBuilder.DbFunction(mathMinMI);

            /* Max */
            var mathMaxMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Max));
            modelBuilder.DbFunction(mathMaxMI);

            /* ABS */
            var absgMI = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Abs));
            modelBuilder.DbFunction(absgMI, b =>
            {
                b.HasSchema("").HasName("abs");
            });
        }
    }
}
