// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

internal class SqlServerWindowAggregateMethods
{
    static SqlServerWindowAggregateMethods()
    {
        var aggMethods = typeof(SqlServerWindowAggregateFunctionExtensions).GetMethods().Where(mi => typeof(IWindowFinal).IsAssignableFrom(mi.GetParameters().FirstOrDefault()?.ParameterType)).ToList();

        CountBigAll = aggMethods.Single(m => m.Name == nameof(SqlServerWindowAggregateFunctionExtensions.CountBig) && m.GetParameters().Length == 1);
        CountBigCol = aggMethods.Single(m => m.Name == nameof(SqlServerWindowAggregateFunctionExtensions.CountBig) && m.GetParameters().Length == 2);

        Stdev = aggMethods.Single(m => m.Name == nameof(SqlServerWindowAggregateFunctionExtensions.Stdev));
        StdevP = aggMethods.Single(m => m.Name == nameof(SqlServerWindowAggregateFunctionExtensions.StdevP));

        Var = aggMethods.Single(m => m.Name == nameof(SqlServerWindowAggregateFunctionExtensions.Var));
        VarP = aggMethods.Single(m => m.Name == nameof(SqlServerWindowAggregateFunctionExtensions.VarP));
    }

    public static MethodInfo CountBigAll { get; }
    public static MethodInfo CountBigCol { get; }

    public static MethodInfo Stdev { get; }
    public static MethodInfo StdevP { get; }

    public static MethodInfo Var { get; }
    public static MethodInfo VarP { get; }
}
