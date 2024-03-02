// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query;

//this is for sqlite
/*/// <summary>
/// todo
/// </summary>
public static class FrameExtensions
{
    /// <summary>
    /// todo
    /// </summary>
    /// <param name="frame">todo</param>
    /// <param name="preceding">todo</param>
    /// <param name="following">todo</param>
    /// <returns>todo</returns>
    public static IWindowFinal Range(this IFrame frame, int preceding, int following)
        => throw new NotImplementedException();
}

/// <summary>
/// todo
/// </summary>
public static class OverExtensions
{
    /// <summary>
    /// todo
    /// </summary>
    /// <param name="frame">todo</param>
    /// <param name="filter">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="NotImplementedException">todo</exception>
    public static IOver Filter(this IOver frame, Func<bool> filter)
        => throw new NotImplementedException();
}

/// <summary>
/// todo
/// </summary>
public static class ExcludeableExtensions
{
    /// <summary>
    /// todo
    /// </summary>
    /// <param name="frame">todo</param>
    /// <param name="exclude">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="NotImplementedException">todo</exception>
    public static IWindowFinal Exclude(this IFrameResults frame, Exclude exclude)
        => throw new NotImplementedException();
}
*/

/// <summary>
/// todo
/// </summary>
public static class WindowFunctionsExtensions
{
    /// <summary>
    /// todo
    /// </summary>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static IOver Over(this DbFunctions _)
    {
        throw new Exception();
    }
}
