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
    internal class SqlServerStringInitializer : IDbFunctionInitializer
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Initialize(ModelBuilder modelBuilder)
        {
            /* Trim */
            var trimMi = typeof(string).GetRuntimeMethod(nameof(string.Trim), new Type[] { });
            modelBuilder.DbFunction(trimMi, b =>
            {
                b.RemoveParameter("trimChars");
                b.Parameter("stringObject").HasParameterIndex(0).IsObjectParameter(true);

                b.TranslateWith((args, dbFunc) =>
                {
                    return new SqlFunctionExpression(
                        "LTRIM",
                        typeof(string),
                        new[]
                        {
                            new SqlFunctionExpression(
                                "RTRIM",
                                typeof(string),
                                new[] { args.ElementAt(0) })
                        });
                });
            });

            /* TrimStart */
            modelBuilder.DbFunction(typeof(string), nameof(string.TrimStart), b =>
            {
                b.HasSchema("").HasName("LTRIM");
                b.RemoveParameter("trimChars");
                b.Parameter("stringObject").HasParameterIndex(0).IsObjectParameter(true);
                b.BeforeInitialization((mc, dbFunc) => ((mc.Arguments[0] as ConstantExpression)?.Value as char[]).Length > 0);
            });

            /* TrimEnd */
            modelBuilder.DbFunction(typeof(string), nameof(string.TrimEnd), b =>
            {
                b.HasSchema("").HasName("RTRIM");
                b.RemoveParameter("trimChars");
                b.Parameter("stringObject").HasParameterIndex(0).IsObjectParameter(true);
                b.BeforeInitialization((mc, dbFunc) => ((mc.Arguments[0] as ConstantExpression)?.Value as char[]).Length > 0);
            });

            /* ToLower */
            var toLowerMi = typeof(string).GetRuntimeMethod(nameof(string.ToLower), new Type[] { });
            modelBuilder.DbFunction(toLowerMi, b =>
            {
                b.HasSchema("").HasName("LOWER");
                b.Parameter("stringObject").HasParameterIndex(0).IsObjectParameter(true);
            });

            /* ToUpper */
            var toUpperMi = typeof(string).GetRuntimeMethod(nameof(string.ToUpper), new Type[] { });
            modelBuilder.DbFunction(toUpperMi, b =>
            {
                b.HasSchema("").HasName("UPPER");
                b.Parameter("stringObject").HasParameterIndex(0).IsObjectParameter(true);
            });

            /* Replace */
            var replaceMi = typeof(string).GetRuntimeMethod(nameof(string.Replace), new[] { typeof(string), typeof(string) });
            modelBuilder.DbFunction(replaceMi, b =>
            {
                b.HasSchema("").HasName("REPLACE");
                b.Parameter("stringObject").HasParameterIndex(0, true).IsObjectParameter(true);
            });

            /* Substring */
            //TODO - add support for no length overload
            var subStringMi = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });
            modelBuilder.DbFunction(subStringMi, b =>
            {
                b.Parameter("stringObject").HasParameterIndex(0, true).IsObjectParameter(true);

                b.TranslateWith((args, dbFunc) =>
                {
                    var newArgs = args.ToList();

                    newArgs[1] = newArgs[1].NodeType == ExpressionType.Constant
                            ? (Expression)Expression.Constant(
                                (int)((ConstantExpression)newArgs[1]).Value + 1)
                            : Expression.Add(
                                newArgs[1],
                                Expression.Constant(1));

                    return new SqlFunctionExpression(
                        "SUBSTRING",
                        typeof(string),
                        newArgs);
                });
            });

            var endsWithMi = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) });
            modelBuilder.DbFunction(endsWithMi, b =>
            {
                b.Parameter("stringObject").HasParameterIndex(0, true).IsObjectParameter(true);

                b.TranslateWith((args, dbFunc) =>
                {
                    var patternExpression = args.ElementAt(1);
                    var patternConstantExpression = patternExpression as ConstantExpression;

                    var endsWithExpression = Expression.Equal(
                        new SqlFunctionExpression(
                            "RIGHT",
                            typeof(string),
                            new[]
                            {
                                args.ElementAt(0),
                                new SqlFunctionExpression("LEN", typeof(int), new[] { patternExpression })
                            }),
                        patternExpression);

                    return new NotNullableExpression(
                        patternConstantExpression != null
                            ? (string)patternConstantExpression.Value == string.Empty
                                ? (Expression)Expression.Constant(true)
                                : endsWithExpression
                            : Expression.OrElse(
                                endsWithExpression,
                                Expression.Equal(patternExpression, Expression.Constant(string.Empty))));
                });
            });

            var startsWithMi = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });
            modelBuilder.DbFunction(startsWithMi, b =>
            {
                var concat = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });

                b.Parameter("stringObject").HasParameterIndex(0, true).IsObjectParameter(true);

                b.TranslateWith((args, dbFunc) =>
                {
                    var patternExpression = args.ElementAt(1);
                    var patternConstantExpression = patternExpression as ConstantExpression;

                    var startsWithExpression = Expression.AndAlso(
                        new LikeExpression(
                            args.ElementAt(0),
                            Expression.Add(patternExpression, Expression.Constant("%", typeof(string)), concat)),
                        Expression.Equal(
                            new SqlFunctionExpression("CHARINDEX", typeof(int), new[] { patternExpression, args.ElementAt(0) }),
                            Expression.Constant(1)));

                    return patternConstantExpression != null
                        ? (string)patternConstantExpression.Value == string.Empty
                            ? (Expression)Expression.Constant(true)
                            : startsWithExpression
                        : Expression.OrElse(
                            startsWithExpression,
                            Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
                });
            });

            modelBuilder.DbFunction(typeof(string), nameof(string.IsNullOrWhiteSpace), b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    return Expression.MakeBinary(
                        ExpressionType.OrElse,
                        new IsNullExpression(args.ElementAt(0)),
                        Expression.Equal(
                            new SqlFunctionExpression(
                                "LTRIM",
                                typeof(string),
                                new[]
                                {
                                new SqlFunctionExpression(
                                    "RTRIM",
                                    typeof(string),
                                    args)
                                }),
                            Expression.Constant("", typeof(string))));
                });
            });

            modelBuilder.DbFunction(typeof(string), nameof(string.Contains), b =>
            {
                b.Parameter("stringObject").HasParameterIndex(0, true).IsObjectParameter(true);

                b.TranslateWith((args, dbFunc) =>
                {
                    var patternExpression = args.ElementAt(1);
                    var patternConstantExpression = patternExpression as ConstantExpression;

                    var charIndexExpression = Expression.GreaterThan(
                        new SqlFunctionExpression("CHARINDEX", typeof(int), new[] { patternExpression, args.ElementAt(0) }),
                        Expression.Constant(0));

                    return
                        patternConstantExpression != null
                            ? (string)patternConstantExpression.Value == string.Empty
                                ? (Expression)Expression.Constant(true)
                                : charIndexExpression
                            : Expression.OrElse(
                                charIndexExpression,
                                Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
                });
            });

            /* ToString */
            const int defaultLength = 100;

            Dictionary<Type, string> typeMappingToString
                    = new Dictionary<Type, string>
                    {
                        { typeof(int), "VARCHAR(11)" },
                        { typeof(long), "VARCHAR(20)" },
                        { typeof(DateTime), $"VARCHAR({defaultLength})" },
                        { typeof(Guid), "VARCHAR(36)" },
                        { typeof(bool), "VARCHAR(5)" },
                        { typeof(byte), "VARCHAR(3)" },
                        { typeof(byte[]), $"VARCHAR({defaultLength})" },
                        { typeof(double), $"VARCHAR({defaultLength})" },
                        { typeof(DateTimeOffset), $"VARCHAR({defaultLength})" },
                        { typeof(char), "VARCHAR(1)" },
                        { typeof(short), "VARCHAR(6)" },
                        { typeof(float), $"VARCHAR({defaultLength})" },
                        { typeof(decimal), $"VARCHAR({defaultLength})" },
                        { typeof(TimeSpan), $"VARCHAR({defaultLength})" },
                        { typeof(uint), "VARCHAR(10)" },
                        { typeof(ushort), "VARCHAR(5)" },
                        { typeof(ulong), "VARCHAR(19)" },
                        { typeof(sbyte), "VARCHAR(4)" }
                    };

            var toStringMis = typeMappingToString.Keys.Select(t => t.GetRuntimeMethod(nameof(object.ToString), new Type[] { }));

            modelBuilder.DbFunction(toStringMis, b =>
            {
                b.TranslateWith((args, dbFunc) =>
                {
                    string storeType;

                    if (typeMappingToString.TryGetValue(
                        args.ElementAt(0).Type
                            .UnwrapNullableType()
                            .UnwrapEnumType(),
                        out storeType))
                    {
                        return new SqlFunctionExpression(
                           functionName: "CONVERT",
                           returnType: dbFunc.ReturnType,
                           arguments: new[]
                           {
                            new SqlFragmentExpression(storeType),
                            args.ElementAt(0)
                           });
                    }

                    return null;
                });

                b.Parameter("stringObject").HasParameterIndex(0, true).IsObjectParameter(true);
            });
        }
    }
}
