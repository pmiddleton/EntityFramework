// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.Query.WindowFunctionsExtensions;

namespace Microsoft.EntityFrameworkCore.Query;

/// <summary>
/// todo
/// </summary>
public interface IWindowFunctionAggregate<T>
{ }

//todo - is this the correct namespace for this?

/// <summary>
/// todo
/// </summary>
public enum RowsPreceding
{
    /// <summary>
    /// todo
    /// </summary>
    CurrentRow,

    /// <summary>
    /// todo
    /// </summary>
    UnboundedPreceding
}

/// <summary>
/// todo
/// </summary>
public enum RowsFollowing
{
    /// <summary>
    /// todo
    /// </summary>
    CurrentRow,

    /// <summary>
    /// todo
    /// </summary>
    UnboundedFollowing
}

/// <summary>
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
    public static IOver Filter(this IFilterableOver frame, Func<bool> filter)
        => throw new NotImplementedException();
}

/// <summary>
/// todo
/// </summary>
public static class WindowFunctionsExtensions
{
   /* /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TSource">todo</typeparam>
    /// <typeparam name="TAggregate">todo</typeparam>
    /// <typeparam name="TPartition">todo</typeparam>
    /// <param name="source">todo</param>
    /// <param name="aggregate">todo</param>
    /// <param name="selector">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static TAggregate? Over1<TSource, TAggregate, TPartition>(this TSource source, Expression<Func<IWindowFunctionAggregate<TSource>, TAggregate>> aggregate, Expression<Func<TSource, TPartition>> selector)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TSource">todo</typeparam>
    /// <typeparam name="TAggregate">todo</typeparam>
    /// <param name="source">todo</param>
    /// <param name="aggregate">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static TAggregate? Over2<TSource, TAggregate>(this TSource source, Expression<Func<IWindowFunctionAggregate<TSource>, TAggregate>> aggregate)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TSource">todo</typeparam>
    /// <typeparam name="TPartition">todo</typeparam>
    /// <param name="aggregate">todo</param>
    /// <param name="partition">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static TSource? Over3<TSource, TPartition>(IWindowFunctionAggregate<TSource> aggregate, TPartition partition)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TResult">todo</typeparam>
    /// <typeparam name="TPartition">todo</typeparam>
    /// <typeparam name="TSource">todo</typeparam>
    /// <param name="source">todo</param>
    /// <param name="aggregate">todo</param>
    /// <param name="selector">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static TResult? Over4<TResult, TPartition, TSource>(TSource source, IWindowFunctionAggregate<TResult> aggregate, Expression<Func<IEnumerable<TSource>, IEnumerable<TPartition>>> selector)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TResult">todo</typeparam>
    /// <typeparam name="TPartition">todo</typeparam>
    /// <typeparam name="TSource">todo</typeparam>ows
    /// <typeparam name="TOrder">todo</typeparam>
    /// <param name="source">todo</param>
    /// <param name="aggregate">todo</param>
    /// <param name="partition">todo</param>
    /// <param name="selector">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static TResult? Over5<TResult, TPartition, TSource, TOrder>(TSource source, IWindowFunctionAggregate<TResult> aggregate, TPartition partition, Expression<Func<IEnumerable<TSource>, IOrderedEnumerable<TOrder>>> selector)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TSource">todo</typeparam>
    /// <typeparam name="TOrder">todo</typeparam>
    /// <param name="source">todo</param>
    /// <param name="selector">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static int Over5Debug<TSource, TOrder>(TSource source, Expression<Func<IQueryable<TSource>, IOrderedQueryable<TOrder>>> selector)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="TSource">todo</typeparam>
    /// <typeparam name="TResult">todo</typeparam>
    /// <param name="source">todo</param>
    /// <param name="selector">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static TResult? Max<TSource, TResult>(this IWindowFunctionAggregate<TSource> source, Expression<Func<TSource, TResult>> selector)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <typeparam name="T">todo</typeparam>
    /// <param name="selector">todo</param>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static IWindowFunctionAggregate<T> Max2<T>(T selector)
    {
        throw new Exception();
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static IWindowFunctionAggregate<int> RowNumber()
    {
        throw new Exception();
    }*/



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
    public interface IWindowFinal
    {
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IFilterableOver : IOver
    { }

    /// <summary>
    /// todo
    /// </summary>
    public interface IOver : IOrderRoot
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <returns>todo</returns>
        IPartition PartitionBy(params object[] partitions);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IPartition : IOrderRoot, IWindowFinal
    {
    }


    /// <summary>
    /// todo
    /// </summary>
    public interface IOrderRoot
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <returns>todo</returns>
        IOrderThen OrderBy(object orderBy);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="orderBy">todo</param>
        /// <returns>todo</returns>
        IOrderThen OrderByDescending(object orderBy);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        IWindowFinal Rows(int preceding);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        IWindowFinal Rows(RowsPreceding preceding);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        /// <param name="following">todo</param>
        IWindowFinal Rows(int preceding, int following);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        /// <param name="following">todo</param>
        IWindowFinal Rows(RowsPreceding preceding, int following);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        /// <param name="following">todo</param>
        IWindowFinal Rows(int preceding, RowsFollowing following);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        /// <param name="following">todo</param>
        IWindowFinal Rows(RowsPreceding preceding, RowsFollowing following);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        IWindowFinal Range(RowsPreceding preceding);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="preceding">todo</param>
        /// <param name="following">todo</param>
        IWindowFinal Range(RowsPreceding preceding, RowsFollowing following);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IOrderThen : IFrame, IWindowFinal
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <param name="orderBy">todo</param>
        /// <returns>todo</returns>
        IOrderThen ThenBy(object orderBy);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="orderBy">todo</param>
        /// <returns>todo</returns>
        IOrderThen ThenByDescending(object orderBy);
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <returns>todo</returns>
    /// <exception cref="Exception">todo</exception>
    public static IFilterableOver Over()
    {
        throw new Exception();
    }
    

}
