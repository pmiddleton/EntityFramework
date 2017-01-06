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
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Microsoft.EntityFrameworkCore.Query.DbFunctions.Internal.Initializers
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqlServerDbFunctionsExtensionsInitializer : IDbFunctionInitializer
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Initialize(ModelBuilder modelBuilder)
        {
            modelBuilder.DbFunction(typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.Left));
            modelBuilder.DbFunction(typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.Right));
            modelBuilder.DbFunction(typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.Reverse));

            /* Truncate */
            var dbTruncates = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.Truncate));
            modelBuilder.DbFunction(dbTruncates, b =>
            {
                b.HasSchema("").HasName("ROUND");
                b.Parameter("function").HasParameterIndex(2).HasValue(1);
            });

            /* AddYears */
            var dbAddYears = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddYears));
            modelBuilder.DbFunction(dbAddYears, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Year);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("dateValue").HasParameterIndex(2);
            });

            /* AddMonths */
            var dbAddMonths = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddMonths));
            modelBuilder.DbFunction(dbAddMonths, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Month);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("dateValue").HasParameterIndex(2);
            });

            /* AddDays */
            var dbAddDays = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddDays));
            modelBuilder.DbFunction(dbAddDays, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Day);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("dateValue").HasParameterIndex(2);
            });

            /* AddHours */
            var dbAddHours = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddHours));
            modelBuilder.DbFunction(dbAddHours, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Hour);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("timeValue").HasParameterIndex(2);
            });

            /* AddMinutes */
            var dbAddMinutes = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddMinutes));
            modelBuilder.DbFunction(dbAddMinutes, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Minute);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("timeValue").HasParameterIndex(2);
            });

            /* AddSeconds */
            var dbAddSeconds = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddSeconds));
            modelBuilder.DbFunction(dbAddSeconds, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Second);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("timeValue").HasParameterIndex(2);
            });

            /* AddMilliseconds */
            var dbAddMilliSeconds = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddMilliseconds));
            modelBuilder.DbFunction(dbAddMilliSeconds, b =>
            {
                b.HasSchema("").HasName("DATEADD");
                b.Parameter("datepart").HasParameterIndex(0).IsIdentifier(true).HasValue(DatePart.Millisecond);
                b.Parameter("addValue").HasParameterIndex(1);
                b.Parameter("timeValue").HasParameterIndex(2);
            });

            /* DiffYears */
            var dbDiffYears = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffYears));
            modelBuilder.DbFunction(dbDiffYears, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Year);
            });

            /* DiffMonths */
            var dbDiffMonths = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffMonths));
            modelBuilder.DbFunction(dbDiffMonths, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Month);
            });

            /* DiffDays */
            var dbDiffDays = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffDays));
            modelBuilder.DbFunction(dbDiffDays, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Day);
            });

            /* DiffHours */
            var dbDiffHours = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffHours));
            modelBuilder.DbFunction(dbDiffHours, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Hour);
            });

            /* DiffMinutes */
            var dbDiffMinutes = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffMinutes));
            modelBuilder.DbFunction(dbDiffMinutes, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Minute);
            });

            /* DiffSeconds */
            var dbDiffSeconds = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffSeconds));
            modelBuilder.DbFunction(dbDiffSeconds, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Second);
            });

            /* DiffMilliSeconds */
            var dbDiffMilliSeconds = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffMilliseconds));
            modelBuilder.DbFunction(dbDiffMilliSeconds, b =>
            {
                b.HasSchema("").HasName("DATEDIFF");
                b.Parameter("datepart").HasParameterIndex(0, true).IsIdentifier(true).HasValue(DatePart.Millisecond);
            });

            /* TruncateTime */
            var dbTruncateTime = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.TruncateTime));
            modelBuilder.DbFunction(dbTruncateTime, b =>
            {
                b.HasSchema("").HasName("CONVERT");

                b.Parameter("data_type").HasParameterIndex(0, true).IsIdentifier(true).HasValue("date");
            });

            /* PatIndex */
            modelBuilder.DbFunction(typeof(SqlServerDbFunctionExtensions), nameof(SqlServerDbFunctionExtensions.PatIndex));

            /* PatIndex */
            modelBuilder.DbFunction(typeof(SqlServerDbFunctionExtensions), nameof(SqlServerDbFunctionExtensions.Like), b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new LikeExpression(args.ElementAt(1), args.ElementAt(0));
                });
            });
        }
    }
}
