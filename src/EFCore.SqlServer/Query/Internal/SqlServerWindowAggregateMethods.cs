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

    }

    public static MethodInfo CountBigAll { get; }
    public static MethodInfo CountBigCol { get; }
}
