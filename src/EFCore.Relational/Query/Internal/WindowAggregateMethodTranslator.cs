// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    /// todo
    /// </summary>
    public class WindowAggregateMethodTranslator : IWindowAggregateMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="sqlExpressionFactory">todo</param>
        public WindowAggregateMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="method">todo</param>
        /// <param name="arguments">todo</param>
        /// <param name="logger">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="Exception">todo</exception>
        public SqlFunctionExpression? Translate(MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            var methodInfo = method.IsGenericMethod
             ? method.GetGenericMethodDefinition()
             : method;

            //todo find better way to make sure we are dealing with the correct method
            //todo - dictionary instead of switch?
            switch (methodInfo.Name)
            {
                case nameof(RelationalWindowAggregateFunctionExtensions.Average)
                    when methodInfo == WindowAggregateMethods.Average:

                    return _sqlExpressionFactory.Function("AVG", arguments, true, new[] { false }, arguments[0].Type, arguments[0].TypeMapping);

                case nameof(RelationalWindowAggregateFunctionExtensions.Count)
                    when methodInfo == WindowAggregateMethods.CountAll:

                    return _sqlExpressionFactory.Function("COUNT", new[] { _sqlExpressionFactory.Fragment("*") }, true, new[] { false }, typeof(int));

                case nameof(RelationalWindowAggregateFunctionExtensions.Count)
                    when methodInfo == WindowAggregateMethods.CountCol:

                    return _sqlExpressionFactory.Function("COUNT", arguments, true, new[] { false }, typeof(int));

                case nameof(RelationalWindowAggregateFunctionExtensions.DenseRank)
                    when methodInfo == WindowAggregateMethods.DenseRank:

                    return _sqlExpressionFactory.Function("DENSE_RANK", Enumerable.Empty<SqlExpression>(), false, new[] { false }, typeof(long));

                case nameof(RelationalWindowAggregateFunctionExtensions.FirstValue)
                    when methodInfo == WindowAggregateMethods.FirstValueOrderThen:

                case nameof(RelationalWindowAggregateFunctionExtensions.FirstValue)
                    when methodInfo == WindowAggregateMethods.FirstValueFrameResults:

                    return _sqlExpressionFactory.Function("FIRST_VALUE", arguments, true, new[] { false }, arguments[0].Type, arguments[0].TypeMapping);

                case nameof(RelationalWindowAggregateFunctionExtensions.LastValue)
                    when methodInfo == WindowAggregateMethods.LastValueOrderThen:

                case nameof(RelationalWindowAggregateFunctionExtensions.LastValue)
                    when methodInfo == WindowAggregateMethods.LastValueFrameResults:

                    return _sqlExpressionFactory.Function("LAST_VALUE", arguments, true, new[] { false }, arguments[0].Type, arguments[0].TypeMapping);

                case nameof(RelationalWindowAggregateFunctionExtensions.Max)
                    when methodInfo == WindowAggregateMethods.Max:

                    return _sqlExpressionFactory.Function("MAX", arguments, true, new[] { false }, arguments[0].Type, arguments[0].TypeMapping);

                case nameof(RelationalWindowAggregateFunctionExtensions.Min)
                    when methodInfo == WindowAggregateMethods.Min:

                    return _sqlExpressionFactory.Function("MIN", arguments, true, new[] { false }, arguments[0].Type, arguments[0].TypeMapping);

                case nameof(RelationalWindowAggregateFunctionExtensions.NTile)
                    when methodInfo == WindowAggregateMethods.NTile:

                    return _sqlExpressionFactory.Function("NTILE", arguments, false, new[] { false }, typeof(long));

                case nameof(RelationalWindowAggregateFunctionExtensions.Rank)
                    when methodInfo == WindowAggregateMethods.Rank:

                    return _sqlExpressionFactory.Function("RANK", Enumerable.Empty<SqlExpression>(), false, new[] { false }, typeof(long));

                case nameof(RelationalWindowAggregateFunctionExtensions.RowNumber)
                    when methodInfo == WindowAggregateMethods.RowNumber:

                    return _sqlExpressionFactory.Function("ROW_NUMBER", Enumerable.Empty<SqlExpression>(), false, new[] { false }, typeof(long));

            }

            return null;
        }
    }
}
