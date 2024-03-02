// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query
{
    internal static class WindowAggregateMethods
    {
        static WindowAggregateMethods()
        {
            var aggMethods = typeof(RelationalWindowAggregateFunctionExtensions).GetMethods().Where(mi => typeof(IWindowFinal).IsAssignableFrom(mi.GetParameters().FirstOrDefault()?.ParameterType)).ToList();

            Max = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Max));
            Min = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Min));
            CountAll = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Count) && m.GetParameters().Length == 1);
            CountCol = aggMethods.Single(m => m.Name == nameof(RelationalWindowAggregateFunctionExtensions.Count) && m.GetParameters().Length == 2);

            Average = aggMethods.Single(m => m.Name == nameof(Average));
        }

        public static MethodInfo Max { get; }
        public static MethodInfo Min { get; }
        public static MethodInfo CountAll { get; }
        public static MethodInfo CountCol { get; }

        public static MethodInfo Average { get; }
    }
}
