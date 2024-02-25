// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.Query.WindowFunctionsExtensions;

namespace Microsoft.EntityFrameworkCore.Query
{
    internal static class WindowAggregateMethods
    {
        static WindowAggregateMethods()
        {
            bool test = typeof(IWindowFinal).IsAssignableFrom(null);

            var aggMethods = typeof(WindowFunctionsExtensions).GetMethods().Where(mi => typeof(IWindowFinal).IsAssignableFrom(mi.GetParameters().FirstOrDefault()?.ParameterType)).ToList();

            Max = aggMethods.Single(m => m.Name == nameof(Max));
        }

        public static MethodInfo Max { get; }
    }
}
