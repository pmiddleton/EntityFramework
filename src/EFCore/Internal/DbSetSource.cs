// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class DbSetSource : IDbSetSource, IDbViewSource
    {
        private static readonly MethodInfo _genericCreateSet
            = typeof(DbSetSource).GetTypeInfo().GetDeclaredMethod(nameof(CreateSetFactory));

        private static readonly MethodInfo _genericCreateView
            = typeof(DbSetSource).GetTypeInfo().GetDeclaredMethod(nameof(CreateViewFactory));

        private readonly ConcurrentDictionary<Type, Func<DbContext, object>> _cache
            = new ConcurrentDictionary<Type, Func<DbContext, object>>();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual object Create(DbContext context, Type type)
            => CreateCore(context, type, _genericCreateSet);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual object CreateView(DbContext context, Type type)
            => CreateCore(context, type, _genericCreateView);

        private object CreateCore(DbContext context, Type type, MethodInfo createMethod)
            => _cache.GetOrAdd(
                type,
                t => (Func<DbContext, object>)createMethod
                    .MakeGenericMethod(t)
                    .Invoke(null, null))(context);

        [UsedImplicitly]
        private static Func<DbContext, object> CreateSetFactory<TEntity>()
            where TEntity : class
            => c => new InternalDbSet<TEntity>(c);

        [UsedImplicitly]
        private static Func<DbContext, DbView<TView>> CreateViewFactory<TView>()
            where TView : class
            => c => new InternalDbView<TView>(c);
    }
}
