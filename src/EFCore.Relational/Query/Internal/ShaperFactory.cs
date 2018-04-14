// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class ShaperFactory
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static Shaper CreateShaper(IMaterializerFactory materializerFactory, IQuerySource querySource, RelationalQueryModelVisitor queryModelVisitor, Type elementType, IEntityType entityType, SelectExpression selectExpression)
        {
            //todo - root qsre does not need materialization - get the RequiresMaterializationExpressionVisitor to ignore it?
            Shaper shaper;

            if (queryModelVisitor.QueryCompilationContext
                    .QuerySourceRequiresMaterialization(querySource)
                || queryModelVisitor.RequiresClientEval)
            {
                var materializerExpression
                    = materializerFactory
                        .CreateMaterializer(
                            entityType,
                            selectExpression,
                            (p, se) =>
                                se.AddToProjection(
                                    p,
                                    querySource),
                            out var typeIndexMap);

                var materializer = materializerExpression.Compile();

                shaper
                    = (Shaper)_createEntityShaperMethodInfo.MakeGenericMethod(elementType)
                        .Invoke(
                            obj: null,
                            parameters: new object[]
                            {
                                querySource,
                                queryModelVisitor.QueryCompilationContext.IsTrackingQuery
                                && !entityType.IsQueryType,
                                entityType.FindPrimaryKey(),
                                materializer,
                                materializerExpression,
                                typeIndexMap,
                                queryModelVisitor.QueryCompilationContext.IsQueryBufferRequired
                                && !entityType.IsQueryType
                            });
            }
            else
            {
                shaper = new ValueBufferShaper(querySource);
            }

            return shaper;
        }

        private static readonly MethodInfo _createEntityShaperMethodInfo
            = typeof(RelationalEntityQueryableExpressionVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateEntityShaper));

        [UsedImplicitly]
        private static IShaper<TEntity> CreateEntityShaper<TEntity>(
            IQuerySource querySource,
            bool trackingQuery,
            IKey key,
            Func<MaterializationContext, object> materializer,
            Expression materializerExpression,
            Dictionary<Type, int[]> typeIndexMap,
            bool useQueryBuffer)
            where TEntity : class
            => !useQueryBuffer
                ? (IShaper<TEntity>)new UnbufferedEntityShaper<TEntity>(
                    querySource,
                    trackingQuery,
                    key,
                    materializer,
                    materializerExpression)
                : new BufferedEntityShaper<TEntity>(
                    querySource,
                    trackingQuery,
                    key,
                    materializer,
                    typeIndexMap);

       
    }
}
