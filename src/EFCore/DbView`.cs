// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     <para>
    ///         A <see cref="DbView{TView}" /> can be used to query instances of <typeparamref name="TView" />.
    ///         LINQ queries against a <see cref="DbView{TView}" /> will be translated into queries against the database.
    ///     </para>
    ///     <para>
    ///         The results of a LINQ query against a <see cref="DbView{TView}" /> will contain the results
    ///         returned from the database and may not reflect changes made in the context that have not
    ///         been persisted to the database. For example, the results will not contain newly added views
    ///         and may still contain views that are marked for deletion.
    ///     </para>
    ///     <para>
    ///         Depending on the database being used, some parts of a LINQ query against a <see cref="DbView{TView}" />
    ///         may be evaluated in memory rather than being translated into a database query.
    ///     </para>
    ///     <para>
    ///         <see cref="DbView{TView}" /> objects are usually obtained from a <see cref="DbView{TView}" />
    ///         property on a derived <see cref="DbContext" /> or from the <see cref="DbContext.Set{TView}" />
    ///         method.
    ///     </para>
    /// </summary>
    /// <typeparam name="TView"> The type of view being operated on by this view. </typeparam>
    public abstract class DbView<TView>
        : IQueryable<TView>, IAsyncEnumerableAccessor<TView>, IInfrastructure<IServiceProvider>
        where TView : class
    {
        /// <summary>
        ///     Returns an <see cref="IEnumerator{T}" /> which when enumerated will execute a query against the database
        ///     to load all views from the database.
        /// </summary>
        /// <returns> The query results. </returns>
        IEnumerator<TView> IEnumerable<TView>.GetEnumerator() => throw new NotImplementedException();

        /// <summary>
        ///     Returns an <see cref="IEnumerator" /> which when enumerated will execute a query against the database
        ///     to load all views from the database.
        /// </summary>
        /// <returns> The query results. </returns>
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        /// <summary>
        ///     Returns an <see cref="IAsyncEnumerable{T}" /> which when enumerated will asynchronously execute the query against
        ///     the database.
        /// </summary>
        /// <returns> The query results. </returns>
        IAsyncEnumerable<TView> IAsyncEnumerableAccessor<TView>.AsyncEnumerable => throw new NotImplementedException();

        /// <summary>
        ///     Gets the IQueryable element type.
        /// </summary>
        Type IQueryable.ElementType => throw new NotImplementedException();

        /// <summary>
        ///     Gets the IQueryable LINQ Expression.
        /// </summary>
        Expression IQueryable.Expression => throw new NotImplementedException();

        /// <summary>
        ///     Gets the IQueryable provider.
        /// </summary>
        IQueryProvider IQueryable.Provider => throw new NotImplementedException();

        /// <summary>
        ///     <para>
        ///         Gets the scoped <see cref="IServiceProvider" /> being used to resolve services.
        ///     </para>
        ///     <para>
        ///         This property is intended for use by extension methods that need to make use of services
        ///         not directly exposed in the public API surface.
        ///     </para>
        /// </summary>
        IServiceProvider IInfrastructure<IServiceProvider>.Instance => throw new NotImplementedException();
    }
}
