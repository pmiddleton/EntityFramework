// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InternalDbView<TView> :
        DbView<TView>, IQueryable<TView>, IAsyncEnumerableAccessor<TView>, IInfrastructure<IServiceProvider>
        where TView : class
    {
        private readonly DbContext _context;
        private IEntityType _entityType;
        private EntityQueryable<TView> _entityQueryable;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public InternalDbView([NotNull] DbContext context)
        {
            Check.NotNull(context, nameof(context));

            // Just storing context/service locator here so that the context will be initialized by the time the
            // set is used and services will be obtained from the correctly scoped container when this happens.
            _context = context;
        }

        private IEntityType EntityType
        {
            get
            {
                _context.CheckDisposed();

                if (_entityType != null)
                {
                    return _entityType;
                }

                _entityType = _context.Model.FindEntityType(typeof(TView));

                if (_entityType == null)
                {
                    throw new InvalidOperationException(CoreStrings.InvalidSetType(typeof(TView).ShortDisplayName()));
                }

                if (!_entityType.IsViewType())
                {
                    _entityType = null;

                    throw new InvalidOperationException(CoreStrings.InvalidSetTypeEntity(typeof(TView).ShortDisplayName()));
                }

                return _entityType;
            }
        }

        private void CheckState()
        {
            // ReSharper disable once AssignmentIsFullyDiscarded
            _ = EntityType;
        }

        private EntityQueryable<TView> EntityQueryable
        {
            get
            {
                CheckState();

                return NonCapturingLazyInitializer.EnsureInitialized(
                    ref _entityQueryable,
                    this,
                    internalSet => internalSet.CreateEntityQueryable());
            }
        }

        private EntityQueryable<TView> CreateEntityQueryable()
            => new EntityQueryable<TView>(_context.GetDependencies().QueryProvider);

        IEnumerator<TView> IEnumerable<TView>.GetEnumerator() => EntityQueryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => EntityQueryable.GetEnumerator();

        IAsyncEnumerable<TView> IAsyncEnumerableAccessor<TView>.AsyncEnumerable => EntityQueryable;

        Type IQueryable.ElementType => EntityQueryable.ElementType;

        Expression IQueryable.Expression => EntityQueryable.Expression;

        IQueryProvider IQueryable.Provider => EntityQueryable.Provider;

        IServiceProvider IInfrastructure<IServiceProvider>.Instance
            => _context.GetInfrastructure();
    }
}
