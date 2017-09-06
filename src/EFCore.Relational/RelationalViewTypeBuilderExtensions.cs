// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Relational database specific extension methods for <see cref="ViewTypeBuilder" />.
    /// </summary>
    public static class RelationalViewTypeBuilderExtensions
    {
        /// <summary>
        ///     Configures the table that the view maps to when targeting a relational database.
        /// </summary>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="name"> The name of the table. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ViewTypeBuilder ToTable(
            [NotNull] this ViewTypeBuilder viewTypeBuilder,
            [CanBeNull] string name)
        {
            Check.NotNull(viewTypeBuilder, nameof(viewTypeBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            viewTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                .Relational(ConfigurationSource.Explicit)
                .ToTable(name);

            return viewTypeBuilder;
        }

        /// <summary>
        ///     Configures the table that the view maps to when targeting a relational database.
        /// </summary>
        /// <typeparam name="TView"> The view type being configured. </typeparam>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="name"> The name of the table. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ViewTypeBuilder<TView> ToTable<TView>(
            [NotNull] this ViewTypeBuilder<TView> viewTypeBuilder,
            [CanBeNull] string name)
            where TView : class
            => (ViewTypeBuilder<TView>)ToTable((ViewTypeBuilder)viewTypeBuilder, name);

        /// <summary>
        ///     Configures the table that the view maps to when targeting a relational database.
        /// </summary>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="name"> The name of the table. </param>
        /// <param name="schema"> The schema of the table. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ViewTypeBuilder ToTable(
            [NotNull] this ViewTypeBuilder viewTypeBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema)
        {
            Check.NotNull(viewTypeBuilder, nameof(viewTypeBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            viewTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                .Relational(ConfigurationSource.Explicit)
                .ToTable(name, schema);

            return viewTypeBuilder;
        }

        /// <summary>
        ///     Configures the table that the view maps to when targeting a relational database.
        /// </summary>
        /// <typeparam name="TView"> The view type being configured. </typeparam>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="name"> The name of the table. </param>
        /// <param name="schema"> The schema of the table. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ViewTypeBuilder<TView> ToTable<TView>(
            [NotNull] this ViewTypeBuilder<TView> viewTypeBuilder,
            [CanBeNull] string name,
            [CanBeNull] string schema)
            where TView : class
            => (ViewTypeBuilder<TView>)ToTable((ViewTypeBuilder)viewTypeBuilder, name, schema);

        /// <summary>
        ///     Configures the discriminator column used to identify which view type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public static DiscriminatorBuilder HasDiscriminator([NotNull] this ViewTypeBuilder viewTypeBuilder)
        {
            Check.NotNull(viewTypeBuilder, nameof(viewTypeBuilder));

            return viewTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                .Relational(ConfigurationSource.Explicit).HasDiscriminator();
        }

        /// <summary>
        ///     Configures the discriminator column used to identify which view type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="name"> The name of the discriminator column. </param>
        /// <param name="discriminatorType"> The type of values stored in the discriminator column. </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public static DiscriminatorBuilder HasDiscriminator(
            [NotNull] this ViewTypeBuilder viewTypeBuilder,
            [NotNull] string name,
            [NotNull] Type discriminatorType)
        {
            Check.NotNull(viewTypeBuilder, nameof(viewTypeBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(discriminatorType, nameof(discriminatorType));

            return viewTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                .Relational(ConfigurationSource.Explicit).HasDiscriminator(name, discriminatorType);
        }

        /// <summary>
        ///     Configures the discriminator column used to identify which view type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <typeparam name="TDiscriminator"> The type of values stored in the discriminator column. </typeparam>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="name"> The name of the discriminator column. </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public static DiscriminatorBuilder<TDiscriminator> HasDiscriminator<TDiscriminator>(
            [NotNull] this ViewTypeBuilder viewTypeBuilder,
            [NotNull] string name)
        {
            Check.NotNull(viewTypeBuilder, nameof(viewTypeBuilder));
            Check.NotEmpty(name, nameof(name));

            return new DiscriminatorBuilder<TDiscriminator>(
                viewTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                    .Relational(ConfigurationSource.Explicit).HasDiscriminator(name, typeof(TDiscriminator)));
        }

        /// <summary>
        ///     Configures the discriminator column used to identify which view type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <typeparam name="TView"> The view type being configured. </typeparam>
        /// <typeparam name="TDiscriminator"> The type of values stored in the discriminator column. </typeparam>
        /// <param name="viewTypeBuilder"> The builder for the view type being configured. </param>
        /// <param name="propertyExpression">
        ///     A lambda expression representing the property to be used as the discriminator (
        ///     <c>blog => blog.Discriminator</c>).
        /// </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public static DiscriminatorBuilder<TDiscriminator> HasDiscriminator<TView, TDiscriminator>(
            [NotNull] this ViewTypeBuilder<TView> viewTypeBuilder,
            [NotNull] Expression<Func<TView, TDiscriminator>> propertyExpression)
            where TView : class
        {
            Check.NotNull(viewTypeBuilder, nameof(viewTypeBuilder));
            Check.NotNull(propertyExpression, nameof(propertyExpression));

            return new DiscriminatorBuilder<TDiscriminator>(
                viewTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                    .Relational(ConfigurationSource.Explicit).HasDiscriminator(propertyExpression.GetPropertyAccess()));
        }
    }
}
