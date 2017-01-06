// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.DbFunctions.Internal.Initializers
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqliteDbFunctionDbFunctionsExtensionsInitializer : IDbFunctionInitializer
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Initialize(ModelBuilder modelBuilder)
        {
            /* Left */
            modelBuilder.DbFunction(typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.Left), b =>
            {
                b.HasName("substr");
                b.Parameter("startIdx").HasParameterIndex(1).HasValue(1);
                b.Parameter("length").HasParameterIndex(2);
            });

            /* Right */
            modelBuilder.DbFunction(typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.Right), b =>
            {
                //TODO - should there be a "negate" option on dbparameter?
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "substr",
                        typeof(string),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.Negate(args.ElementAt(1))
                        });
                });
            });

            var concatMi = typeof(string).GetTypeInfo().GetDeclaredMethods(nameof(string.Concat))
                            .Where(mi =>
                            {
                                var parameters = mi.GetParameters();

                                return parameters.Length == 2 && parameters[0].ParameterType == typeof(object)
                                        && parameters[1].ParameterType == typeof(object);
                            }).Single();

            /* AddYears */
            var dbAddYears = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddYears));
            modelBuilder.DbFunction(dbAddYears, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "datetime",
                        typeof(DateTime?),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.MakeBinary(ExpressionType.Add,
                                   Expression.Convert(args.ElementAt(1), typeof(object)),
                                   Expression.Constant(" year"),
                                  false,
                                 concatMi) as Expression
                        });
                });
            });


            /* AddMonths */
            var dbAddMonths = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddMonths));
            modelBuilder.DbFunction(dbAddMonths, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "datetime",
                        typeof(DateTime?),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.MakeBinary(ExpressionType.Add,
                                   Expression.Convert(args.ElementAt(1), typeof(object)),
                                   Expression.Constant(" month"),
                                  false,
                                 concatMi) as Expression
                        });
                });
            });

            /* AddDays */
            var dbAddDays = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddDays));
            modelBuilder.DbFunction(dbAddDays, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "datetime",
                        typeof(DateTime?),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.MakeBinary(ExpressionType.Add,
                                   Expression.Convert(args.ElementAt(1), typeof(object)),
                                   Expression.Constant(" day"),
                                  false,
                                 concatMi) as Expression
                        });
                });
            });

            /* AddHours */
            var dbAddHours = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddHours));
            modelBuilder.DbFunction(dbAddHours, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "datetime",
                        typeof(DateTime?),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.MakeBinary(ExpressionType.Add,
                                   Expression.Convert(args.ElementAt(1), typeof(object)),
                                   Expression.Constant(" hour"),
                                  false,
                                 concatMi) as Expression
                        });
                });
            });


            /* AddMinutes */
            var dbAddMinutes = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddMinutes));
            modelBuilder.DbFunction(dbAddMinutes, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "datetime",
                        typeof(DateTime?),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.MakeBinary(ExpressionType.Add,
                                   Expression.Convert(args.ElementAt(1), typeof(object)),
                                   Expression.Constant(" minute"),
                                  false,
                                 concatMi) as Expression
                        });
                });
            });

            /* AddSeconds */
            var dbAddSeconds = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.AddSeconds));
            modelBuilder.DbFunction(dbAddSeconds, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "datetime",
                        typeof(DateTime?),
                        new[]
                        {
                             args.ElementAt(0),
                             Expression.MakeBinary(ExpressionType.Add,
                                   Expression.Convert(args.ElementAt(1), typeof(object)),
                                   Expression.Constant(" second"),
                                  false,
                                 concatMi) as Expression
                        });
                });
            });


            /* DiffDays */
            var dbDiffDays = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffDays));
            modelBuilder.DbFunction(dbDiffDays, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return Expression.Negate(
                            Expression.MakeBinary(ExpressionType.Subtract,
                            new SqlFunctionExpression(
                                "julianday",
                                typeof(int),
                                new[]
                                {
                                     args.ElementAt(0)
                                }),
                            new SqlFunctionExpression(
                                "julianday",
                                typeof(int),
                                new[]
                                {
                                     args.ElementAt(1)
                                })
                            )
                        );
                });
            });

            /* DiffHours */
            var dbDiffHours = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffHours));
            modelBuilder.DbFunction(dbDiffHours, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return Expression.Negate(
                            Expression.Multiply(
                                Expression.MakeBinary(ExpressionType.Subtract,
                                    new SqlFunctionExpression(
                                        "julianday",
                                        typeof(int),
                                        new[]
                                        {
                                             args.ElementAt(0)
                                        }),
                                    new SqlFunctionExpression(
                                        "julianday",
                                        typeof(int),
                                        new[]
                                        {
                                             args.ElementAt(1)
                                        })
                                    ),
                                Expression.Constant(24)
                            )
                        );
                });
            });

            /* DiffMinutes */
            var dbDiffMinutes = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffMinutes));
            modelBuilder.DbFunction(dbDiffMinutes, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return Expression.Negate(
                            Expression.Multiply(
                                Expression.MakeBinary(ExpressionType.Subtract,
                                    new SqlFunctionExpression(
                                        "julianday",
                                        typeof(int),
                                        new[]
                                        {
                                             args.ElementAt(0)
                                        }),
                                    new SqlFunctionExpression(
                                        "julianday",
                                        typeof(int),
                                        new[]
                                        {
                                             args.ElementAt(1)
                                        })
                                    ),
                                Expression.Constant(1440)
                            )
                        );
                });
            });

            /* DiffSeconds */
            var dbDiffSeconds = typeof(DbFunctionsExtensions).GetTypeInfo().GetDeclaredMethods(nameof(DbFunctionsExtensions.DiffSeconds));
            modelBuilder.DbFunction(dbDiffSeconds, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return Expression.Negate(
                            Expression.Multiply(
                                Expression.MakeBinary(ExpressionType.Subtract,
                                    new SqlFunctionExpression(
                                        "julianday",
                                        typeof(int),
                                        new[]
                                        {
                                             args.ElementAt(0)
                                        }),
                                    new SqlFunctionExpression(
                                        "julianday",
                                        typeof(int),
                                        new[]
                                        {
                                             args.ElementAt(1)
                                        })
                                    ),
                                Expression.Constant(86400)
                            )
                        );
                });
            });

            /* TruncateTime */
            modelBuilder.DbFunction(typeof(DbFunctionsExtensions), nameof(DbFunctionsExtensions.TruncateTime)).HasName("date");
        }
    }
}
