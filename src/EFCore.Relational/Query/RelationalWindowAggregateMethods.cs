// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query;

internal static class RelationalWindowAggregateMethods
{
    static RelationalWindowAggregateMethods()
    {
        var aggMethods = typeof(RelationalWindowAggregateFunctionExtensions).GetMethods().Where(mi => typeof(IWindowFinal).IsAssignableFrom(mi.GetParameters().FirstOrDefault()?.ParameterType)).ToList();

        Average = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Average));
        CountAll = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Count) && m.GetParameters().Length == 1);
        CountCol = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Count) && m.GetParameters().Length == 2);
        CumeDist = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.CumeDist));
        DenseRank = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.DenseRank));
        FirstValueFrameResults = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.FirstValue) && typeof(IFrameResults).IsAssignableFrom(m.GetParameters().FirstOrDefault()?.ParameterType));
        FirstValueOrderThen = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.FirstValue) && typeof(IOrderThen).IsAssignableFrom(m.GetParameters().FirstOrDefault()?.ParameterType));
        Lag = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Lag));
        LastValueFrameResults = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.LastValue) && typeof(IFrameResults).IsAssignableFrom(m.GetParameters().FirstOrDefault()?.ParameterType));
        LastValueOrderThen = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.LastValue) && typeof(IOrderThen).IsAssignableFrom(m.GetParameters().FirstOrDefault()?.ParameterType));
        Lead = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Lead));
        Max = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Max));
        Min = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Min));
        NTile = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.NTile));
        PercentRank = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.PercentRank));
        Rank = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Rank));
        RowNumber = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.RowNumber));
        Sum = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Sum));
    }

    public static MethodInfo Average { get; }
    public static MethodInfo CountAll { get; }
    public static MethodInfo CountCol { get; }
    public static MethodInfo CumeDist { get; }
    public static MethodInfo DenseRank { get; }
    public static MethodInfo FirstValueFrameResults { get; }
    public static MethodInfo FirstValueOrderThen { get; }
    public static MethodInfo Lag { get; }
    public static MethodInfo Lead { get; }
    public static MethodInfo LastValueFrameResults { get; }
    public static MethodInfo LastValueOrderThen { get; }
    public static MethodInfo Max { get; }
    public static MethodInfo Min { get; }
    public static MethodInfo NTile { get; }
    public static MethodInfo PercentRank { get; }
    public static MethodInfo Rank { get; }
    public static MethodInfo RowNumber { get; }
    public static MethodInfo Sum { get; }
}
