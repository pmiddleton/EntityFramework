// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class SqlServerWindowAggregateMethodTranslator : IWindowAggregateMethodCallTranslator
{
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public SqlServerWindowAggregateMethodTranslator(ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public SqlFunctionExpression? Translate(MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        var methodInfo = method.IsGenericMethod
        ? method.GetGenericMethodDefinition()
        : method;

        switch (methodInfo.Name)
        {
            case nameof(SqlServerWindowAggregateFunctionExtensions.CountBig)
                when methodInfo == SqlServerWindowAggregateMethods.CountBigAll:

                return _sqlExpressionFactory.Function("COUNT_BIG", new[] { _sqlExpressionFactory.Fragment("*") }, true, [false], typeof(long));

            case nameof(SqlServerWindowAggregateFunctionExtensions.CountBig)
                when methodInfo == SqlServerWindowAggregateMethods.CountBigAllFilter:

                return _sqlExpressionFactory.Function("COUNT_BIG", BuildCaseExpression(_sqlExpressionFactory.Constant("1")), true, [false], typeof(long));

            case nameof(SqlServerWindowAggregateFunctionExtensions.CountBig)
                when methodInfo == SqlServerWindowAggregateMethods.CountBigCol:

                return _sqlExpressionFactory.Function("COUNT_BIG", arguments, true, [false], typeof(long));

            case nameof(SqlServerWindowAggregateFunctionExtensions.CountBig)
                when methodInfo == SqlServerWindowAggregateMethods.CountBigColFilter:

                return _sqlExpressionFactory.Function("COUNT_BIG", BuildCaseExpression(), true, [false], typeof(long));

            case nameof(SqlServerWindowAggregateFunctionExtensions.Stdev)
                when methodInfo == SqlServerWindowAggregateMethods.Stdev:

            case nameof(SqlServerWindowAggregateFunctionExtensions.Stdev)
                when methodInfo == SqlServerWindowAggregateMethods.Stdev:

                return _sqlExpressionFactory.Function("STDEV", arguments, true, [false], typeof(double));

            case nameof(SqlServerWindowAggregateFunctionExtensions.StdevP)
                when methodInfo == SqlServerWindowAggregateMethods.StdevP:

                return _sqlExpressionFactory.Function("STDEVP", arguments, false, [false], typeof(double));

            case nameof(SqlServerWindowAggregateFunctionExtensions.Var)
                when methodInfo == SqlServerWindowAggregateMethods.Var:

                return _sqlExpressionFactory.Function("VAR", arguments, true, [false], typeof(double));

            case nameof(SqlServerWindowAggregateFunctionExtensions.VarP)
                when methodInfo == SqlServerWindowAggregateMethods.VarP:

                return _sqlExpressionFactory.Function("VARP", arguments, false, [false], typeof(double));
        }

        return null;

        CaseExpression[] BuildCaseExpression(SqlExpression? result = null)
         => [_sqlExpressionFactory.Case([new CaseWhenClause(arguments[result == null ? 1 : 0], result ?? arguments[0])], _sqlExpressionFactory.Constant(null))];

    }
}
