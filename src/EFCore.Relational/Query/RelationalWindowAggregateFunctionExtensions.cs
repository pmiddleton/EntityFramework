// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    /// todo
    /// </summary>
    public static class RelationalWindowAggregateFunctionExtensions
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="TSource">todo</typeparam>
        /// <param name="partition">todo</param>
        /// <param name="source">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="Exception">todo</exception>
        public static TSource? Max<TSource>(this IWindowFinal partition, TSource source)
        {
            throw new Exception();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="TSource">todo</typeparam>
        /// <param name="partition">todo</param>
        /// <param name="source">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="Exception">todo</exception>
        public static TSource? Min<TSource>(this IWindowFinal partition, TSource source)
        {
            throw new Exception();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="partition">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="Exception">todo</exception>
        public static int? Count(this IWindowFinal partition)
        {
            throw new Exception();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="TSource">todo</typeparam>
        /// <param name="partition">todo</param>
        /// <param name="source">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="Exception">todo</exception>
        public static int? Count<TSource>(this IWindowFinal partition, TSource source)
        {
            throw new Exception();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="TSource">todo</typeparam>
        /// <param name="partition">todo</param>
        /// <param name="source">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="Exception">todo</exception>
        public static TSource? Average<TSource>(this IWindowFinal partition, TSource source)
        {
            throw new Exception();
        }
    }
}
