// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class DbFunction : IMutableDbFunction, IConventionDbFunction
    {
        private readonly IMutableModel _model;
        private readonly string _annotationName;
        private readonly List<DbFunctionParameter> _parameters;
        private string _schema;
        private string _name;
        private string _storeType;
        private RelationalTypeMapping _typeMapping;
        private Func<IReadOnlyCollection<SqlExpression>, SqlExpression> _translation;

        private ConfigurationSource? _schemaConfigurationSource;
        private ConfigurationSource? _nameConfigurationSource;
        private ConfigurationSource? _storeTypeConfigurationSource;
        private ConfigurationSource? _typeMappingConfigurationSource;
        private ConfigurationSource? _translationConfigurationSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public DbFunction(
            [NotNull] MethodInfo methodInfo,
            [NotNull] IMutableModel model,
            ConfigurationSource configurationSource)
        {
            Check.NotNull(methodInfo, nameof(methodInfo));
            Check.NotNull(model, nameof(model));

            if (methodInfo.IsGenericMethod)
            {
                throw new ArgumentException(RelationalStrings.DbFunctionGenericMethodNotSupported(methodInfo.DisplayName()));
            }

            if (!methodInfo.IsStatic
                && !typeof(DbContext).IsAssignableFrom(methodInfo.DeclaringType))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new ArgumentException(
                    RelationalStrings.DbFunctionInvalidInstanceType(
                        methodInfo.DisplayName(), methodInfo.DeclaringType.ShortDisplayName()));
            }

            if (methodInfo.ReturnType == null
                || methodInfo.ReturnType == typeof(void))
            {
                throw new ArgumentException(
                    RelationalStrings.DbFunctionInvalidReturnType(
                        methodInfo.DisplayName(), methodInfo.ReturnType.ShortDisplayName()));
            }

            if (methodInfo.ReturnType.IsGenericType
                && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                IsIQueryable = true;

                //todo - if the generic argument is not usuable as an entitytype should we throw here?  IE IQueryable<int>
                //the built in entitytype will throw is the type is not a class
                if (model.FindEntityType(methodInfo.ReturnType.GetGenericArguments()[0]) == null)
                {
                    model.AddEntityType(methodInfo.ReturnType.GetGenericArguments()[0]).IsKeyless = true;
                }
            }

            MethodInfo = methodInfo;

            _model = model;

            _parameters = methodInfo.GetParameters()
                .Select((pi, i) => new DbFunctionParameter(this, pi.Name, pi.ParameterType))
                .ToList();

            _annotationName = BuildAnnotationName(methodInfo);
            if (configurationSource == ConfigurationSource.Explicit)
            {
                _model.AddAnnotation(_annotationName, this);
            }
            else
            {
                ((IConventionModel)_model).AddAnnotation(
                    _annotationName,
                    this,
                    configurationSource == ConfigurationSource.DataAnnotation);
            }
        }

        /// <summary>
        ///     The builder that can be used to configure this function.
        /// </summary>
        public virtual IConventionDbFunctionBuilder Builder => new DbFunctionBuilder(this);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static IEnumerable<DbFunction> GetDbFunctions([NotNull] Model model)
        {
            Check.NotNull(model, nameof(model));

            return model.GetAnnotations()
                .Where(a => a.Name.StartsWith(RelationalAnnotationNames.DbFunction, StringComparison.Ordinal))
                .Select(a => a.Value)
                .Cast<DbFunction>();
        }

        private static string BuildAnnotationName(MethodBase methodBase)
            =>
                // ReSharper disable once AssignNullToNotNullAttribute
                // ReSharper disable once PossibleNullReferenceException
                $"{RelationalAnnotationNames.DbFunction}{methodBase.DeclaringType.FullName}{methodBase.Name}({string.Join(",", methodBase.GetParameters().Select(p => p.ParameterType.FullName))})";

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConfigurationSource GetConfigurationSource()
            => ((IConventionModel)_model).FindAnnotation(_annotationName).GetConfigurationSource();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void UpdateConfigurationSource(ConfigurationSource configurationSource)
            => ((Model)_model).FindAnnotation(_annotationName).UpdateConfigurationSource(configurationSource);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string Schema
        {
            get => _schema ?? _model.GetDefaultSchema();
            set => SetSchema(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void SetSchema([CanBeNull] string schema, ConfigurationSource configurationSource)
        {
            _schema = schema;

            UpdateSchemaConfigurationSource(configurationSource);
        }

        private void UpdateSchemaConfigurationSource(ConfigurationSource configurationSource)
            => _schemaConfigurationSource = configurationSource.Max(_schemaConfigurationSource);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConfigurationSource? GetSchemaConfigurationSource() => _schemaConfigurationSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string Name
        {
            get => _name ?? MethodInfo.Name;
            set => SetName(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void SetName([NotNull] string name, ConfigurationSource configurationSource)
        {
            Check.NotNull(name, nameof(name));

            _name = name;

            UpdateNameConfigurationSource(configurationSource);
        }

        private void UpdateNameConfigurationSource(ConfigurationSource configurationSource)
            => _nameConfigurationSource = configurationSource.Max(_nameConfigurationSource);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConfigurationSource? GetNameConfigurationSource() => _nameConfigurationSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual MethodInfo MethodInfo { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string StoreType
        {
            get => _storeType;
            set => SetStoreType(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void SetStoreType([NotNull] string storeType, ConfigurationSource configurationSource)
        {
            Check.NotNull(storeType, nameof(storeType));

            _storeType = storeType;

            UpdateStoreTypeConfigurationSource(configurationSource);
        }

        private void UpdateStoreTypeConfigurationSource(ConfigurationSource configurationSource)
            => _storeTypeConfigurationSource = configurationSource.Max(_storeTypeConfigurationSource);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConfigurationSource? GetStoreTypeConfigurationSource() => _storeTypeConfigurationSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual RelationalTypeMapping TypeMapping
        {
            get => _typeMapping;
            set => SetTypeMapping(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void SetTypeMapping([CanBeNull] RelationalTypeMapping typeMapping, ConfigurationSource configurationSource)
        {
            _typeMapping = typeMapping;

            UpdateTypeMappingConfigurationSource(configurationSource);
        }

        private void UpdateTypeMappingConfigurationSource(ConfigurationSource configurationSource)
            => _typeMappingConfigurationSource = configurationSource.Max(_typeMappingConfigurationSource);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConfigurationSource? GetTypeMappingConfigurationSource() => _typeMappingConfigurationSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual Func<IReadOnlyCollection<SqlExpression>, SqlExpression> Translation
        {
            get => _translation;
            set => SetTranslation(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual bool IsIQueryable { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void SetTranslation(
            [CanBeNull] Func<IReadOnlyCollection<SqlExpression>, SqlExpression> translation,
            ConfigurationSource configurationSource)
        {
            _translation = translation;

            UpdateTranslationConfigurationSource(configurationSource);
        }

        private void UpdateTranslationConfigurationSource(ConfigurationSource configurationSource)
            => _translationConfigurationSource = configurationSource.Max(_translationConfigurationSource);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConfigurationSource? GetTranslationConfigurationSource() => _translationConfigurationSource;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static DbFunction FindDbFunction(
            [NotNull] IModel model,
            [NotNull] MethodInfo methodInfo)
        {
            var dbFunction = model[BuildAnnotationName(methodInfo)] as DbFunction;

            if(dbFunction == null
                && methodInfo.GetParameters().Any(p => p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)))
            {
                var parameters = methodInfo.GetParameters().Select(p => p.ParameterType.IsGenericType
                                        && p.ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)
                                        && p.ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<>)
                                    ? p.ParameterType.GetGenericArguments()[0].GetGenericArguments()[0]
                                    : p.ParameterType).ToArray();

                var nonExpressionMethod = methodInfo.DeclaringType.GetMethod(methodInfo.Name, parameters);

                dbFunction =  nonExpressionMethod != null
                                ? model[BuildAnnotationName(nonExpressionMethod)] as DbFunction
                                : null;
            }

            return dbFunction;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static DbFunction RemoveDbFunction(
            [NotNull] IMutableModel model,
            [NotNull] MethodInfo methodInfo)
            => model.RemoveAnnotation(BuildAnnotationName(methodInfo))?.Value as DbFunction;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        IModel IDbFunction.Model => _model;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        IMutableModel IMutableDbFunction.Model => _model;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        IConventionModel IConventionDbFunction.Model => (IConventionModel)_model;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        void IConventionDbFunction.SetName(string name, bool fromDataAnnotation)
            => SetName(name, fromDataAnnotation ? ConfigurationSource.DataAnnotation : ConfigurationSource.Convention);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        void IConventionDbFunction.SetSchema(string schema, bool fromDataAnnotation)
            => SetSchema(schema, fromDataAnnotation ? ConfigurationSource.DataAnnotation : ConfigurationSource.Convention);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        void IConventionDbFunction.SetStoreType(string storeType, bool fromDataAnnotation)
            => SetStoreType(storeType, fromDataAnnotation ? ConfigurationSource.DataAnnotation : ConfigurationSource.Convention);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        void IConventionDbFunction.SetTypeMapping(RelationalTypeMapping returnTypeMapping, bool fromDataAnnotation)
            => SetTypeMapping(returnTypeMapping, fromDataAnnotation ? ConfigurationSource.DataAnnotation : ConfigurationSource.Convention);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        void IConventionDbFunction.SetTranslation(
            Func<IReadOnlyCollection<SqlExpression>, SqlExpression> translation, bool fromDataAnnotation)
            => SetTranslation(translation, fromDataAnnotation ? ConfigurationSource.DataAnnotation : ConfigurationSource.Convention);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IReadOnlyList<IDbFunctionParameter> Parameters => _parameters;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        IReadOnlyList<IConventionDbFunctionParameter> IConventionDbFunction.Parameters => _parameters;
    }
}
