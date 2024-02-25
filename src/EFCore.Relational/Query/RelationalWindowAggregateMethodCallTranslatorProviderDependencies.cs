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
    public sealed class RelationalWindowAggregateMethodCallTranslatorProviderDependencies
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <param name="sqlExpressionFactory">todo</param>
        /// <param name="plugins">todo</param>
        /// <param name="typeMappingSource">todo</param>
        [EntityFrameworkInternal]
        public RelationalWindowAggregateMethodCallTranslatorProviderDependencies(
            ISqlExpressionFactory sqlExpressionFactory,
            IEnumerable<IWindowAggregateMethodCallTranslatorPlugin> plugins,
            IRelationalTypeMappingSource typeMappingSource)
        {
            SqlExpressionFactory = sqlExpressionFactory;
            Plugins = plugins;
            RelationalTypeMappingSource = typeMappingSource;
        }

        /// <summary>
        ///     The expression factory..
        /// </summary>
        public ISqlExpressionFactory SqlExpressionFactory { get; init; }

        /// <summary>
        ///     Registered plugins.
        /// </summary>
        public IEnumerable<IWindowAggregateMethodCallTranslatorPlugin> Plugins { get; init; }

        /// <summary>
        ///     Relational Type Mapping Source.
        /// </summary>
        public IRelationalTypeMappingSource RelationalTypeMappingSource { get; init; }
    }
}
