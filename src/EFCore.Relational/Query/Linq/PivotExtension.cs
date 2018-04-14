// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{
    /// <summary>
    /// todo
    /// </summary>
    public static class PivotExtension
    {
        private static MethodInfo _pivotMethodInfo = typeof(PivotExtension).GetMethods().Single(m => m.Name == nameof(Pivot) && m.GetParameters().Length == 4);
            
        /// <summary>
        /// todo
        /// </summary>
        public static IQueryable<TResult> Pivot<TSource, TResult>(this IQueryable<TSource> source,
                                                                            Expression<Func<TSource, object>> pivotSelector,
                                                                            Expression<Func<IQueryable<TSource>, object>> aggregate)
        {
            if (source == null)
            { 
                throw new Exception("todo");
            }

            if (pivotSelector == null)
            {
                throw new Exception("todo");
            }

            if (aggregate == null)
            {
                throw new Exception("todo");
            }

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    _pivotMethodInfo
                    .MakeGenericMethod(typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(pivotSelector),
                    Expression.Quote(aggregate),
                    Expression.Lambda(Expression.New(typeof(TResult)),
                                        Expression.Parameter(typeof(TSource), "s"))));
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="TSource">todo</typeparam>
        /// <typeparam name="TResult">todo</typeparam>
        /// <param name="source">todo</param>
        /// <param name="pivotSelector">todo</param>
        /// <param name="aggregate">todo</param>
        /// <param name="selector">todo</param>
        /// <returns>todo</returns>
        //note - this overload is needed so the TSource can be a anonymous type.  
        public static IQueryable<TResult> Pivot<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, object>> pivotSelector,
            Expression<Func<IQueryable<TSource>, object>> aggregate,
            Expression<Func<TSource, TResult>> selector)
        {
            if (source == null)
            {
                throw new Exception("todo");
            }

            if (pivotSelector == null)
            {
                throw new Exception("todo");
            }

            if (aggregate == null)
            {
                throw new Exception("todo");
            }

            if (selector == null)
            {
                throw new Exception("todo");
            }

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod())
                    .MakeGenericMethod(typeof(TSource), typeof(TResult)),
                    source.Expression,
                    Expression.Quote(pivotSelector),
                    Expression.Quote(aggregate),
                    Expression.Quote(selector)));
        }
    }
}
