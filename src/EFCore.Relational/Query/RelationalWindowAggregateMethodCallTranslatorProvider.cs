// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    /// todo
    /// </summary>
    public class RelationalWindowAggregateMethodCallTranslatorProvider : IWindowAggregateMethodCallTranslatorProvider
    {
        private readonly List<IWindowAggregateMethodCallTranslator> _plugins = new();
        private readonly List<IWindowAggregateMethodCallTranslator> _translators = new();

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="dependencies">todo</param>
        public RelationalWindowAggregateMethodCallTranslatorProvider(RelationalWindowAggregateMethodCallTranslatorProviderDependencies dependencies)
        {
            Dependencies = dependencies;

            _plugins.AddRange(dependencies.Plugins.SelectMany(p => p.Translators));

            var sqlExpressionFactory = dependencies.SqlExpressionFactory;

            _translators.AddRange(
                new IWindowAggregateMethodCallTranslator[] { new WindowAggregateMethodTranslator(sqlExpressionFactory) });
        }

        /// <summary>
        ///     Dependencies for this service.
        /// </summary>
        protected virtual RelationalWindowAggregateMethodCallTranslatorProviderDependencies Dependencies { get; }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="model">todo</param>
        /// <param name="method">todo</param>
        /// <param name="arguments">todo</param>
        /// <param name="logger">todo</param>
        /// <returns>todo</returns>
        /// <exception cref="NotImplementedException">todo</exception>
        public SqlExpression? Translate(IModel model, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
           => _plugins.Concat(_translators)
            .Select(t => t.Translate(method, arguments, logger))
            .FirstOrDefault(t => t != null);

        /// <summary>
        ///     Adds additional translators which will take priority over existing registered translators.
        /// </summary>
        /// <param name="translators">Translators to add.</param>
        protected virtual void AddTranslators(IEnumerable<IWindowAggregateMethodCallTranslator> translators)
            => _translators.InsertRange(0, translators);
    }
}
