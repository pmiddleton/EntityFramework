using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    /// <summary>
    ///     <para>
    ///         Provides a simple API for configuring an <see cref="EntityType" />.
    ///     </para>
    ///     <para>
    ///         Instances of this class are returned from methods when using the <see cref="ModelBuilder" /> API
    ///         and it is not designed to be directly constructed in your application code.
    ///     </para>
    /// </summary>
    /// <typeparam name="TView"> The view type being configured. </typeparam>
    public class ViewTypeBuilder<TView> : ViewTypeBuilder
        where TView : class
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public ViewTypeBuilder([NotNull] InternalEntityTypeBuilder builder)
            : base(builder)
        {
        }

        private InternalEntityTypeBuilder Builder => this.GetInfrastructure<InternalEntityTypeBuilder>();

        /// <summary>
        ///     Adds or updates an annotation on the view type. If an annotation with the key specified in
        ///     <paramref name="annotation" /> already exists its value will be updated.
        /// </summary>
        /// <param name="annotation"> The key of the annotation to be added or updated. </param>
        /// <param name="value"> The value to be stored in the annotation. </param>
        /// <returns> The same typeBuilder instance so that multiple configuration calls can be chained. </returns>
        public new virtual ViewTypeBuilder<TView> HasAnnotation([NotNull] string annotation, [NotNull] object value) =>
            (ViewTypeBuilder<TView>)base.HasAnnotation(annotation, value);

        /// <summary>
        ///     Sets the base type of this view in an inheritance hierarchy.
        /// </summary>
        /// <param name="name"> The name of the base type. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public new virtual ViewTypeBuilder<TView> HasBaseType([CanBeNull] string name) =>
            new ViewTypeBuilder<TView>(Builder.HasBaseType(name, ConfigurationSource.Explicit));

        /// <summary>
        ///     Sets the base type of this view in an inheritance hierarchy.
        /// </summary>
        /// <param name="viewType"> The base type. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public new virtual ViewTypeBuilder<TView> HasBaseType([CanBeNull] Type viewType) =>
            new ViewTypeBuilder<TView>(Builder.HasBaseType(viewType, ConfigurationSource.Explicit));

        /// <summary>
        ///     Sets the base type of this view in an inheritance hierarchy.
        /// </summary>
        /// <typeparam name="TBaseType"> The base type. </typeparam>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ViewTypeBuilder<TView> HasBaseType<TBaseType>() => HasBaseType(typeof(TBaseType));

        /// <summary>
        ///     Returns an object that can be used to configure a property of the view type.
        ///     If the specified property is not already part of the model, it will be added.
        /// </summary>
        /// <param name="propertyExpression">
        ///     A lambda expression representing the property to be configured (
        ///     <c>blog => blog.Url</c>).
        /// </param>
        /// <returns> An object that can be used to configure the property. </returns>
        public virtual PropertyBuilder<TProperty> Property<TProperty>(
            [NotNull] Expression<Func<TView, TProperty>> propertyExpression) => new PropertyBuilder<TProperty>(
            Builder.Property(
                Check.NotNull(propertyExpression, nameof(propertyExpression)).GetPropertyAccess(),
                ConfigurationSource.Explicit));

        /// <summary>
        ///     Excludes the given property from the view type. This method is typically used to remove properties
        ///     from the view type that were added by convention.
        /// </summary>
        /// <param name="propertyExpression">
        ///     A lambda expression representing the property to be ignored
        ///     (<c>blog => blog.Url</c>).
        /// </param>
        public virtual ViewTypeBuilder<TView> Ignore([NotNull] Expression<Func<TView, object>> propertyExpression) =>
            (ViewTypeBuilder<TView>)base.Ignore(
                Check.NotNull(propertyExpression, nameof(propertyExpression)).GetPropertyAccess().Name);

        /// <summary>
        ///     Excludes the given property from the view type. This method is typically used to remove properties
        ///     from the view type that were added by convention.
        /// </summary>
        /// <param name="propertyName"> The name of then property to be removed from the view type. </param>
        public new virtual ViewTypeBuilder<TView> Ignore([NotNull] string propertyName) =>
            (ViewTypeBuilder<TView>)base.Ignore(propertyName);

        /// <summary>
        ///     Specifies a LINQ predicate expression that will automatically be applied to any queries targeting
        ///     this view type.
        /// </summary>
        /// <param name="filter">The LINQ predicate expression.</param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ViewTypeBuilder<TView> HasQueryFilter([CanBeNull] Expression<Func<TView, bool>> filter) =>
            (ViewTypeBuilder<TView>)base.HasQueryFilter(filter);

        /// <summary>
        ///     Configures an index on the specified properties. If there is an existing index on the given
        ///     set of properties, then the existing index will be returned for configuration.
        /// </summary>
        /// <param name="indexExpression">
        ///     <para>
        ///         A lambda expression representing the property(s) to be included in the index
        ///         (<c>blog => blog.Url</c>).
        ///     </para>
        ///     <para>
        ///         If the index is made up of multiple properties then specify an anonymous type including the
        ///         properties (<c>post => new { post.Title, post.BlogId }</c>).
        ///     </para>
        /// </param>
        /// <returns> An object that can be used to configure the index. </returns>
        public virtual IndexBuilder HasIndex([NotNull] Expression<Func<TView, object>> indexExpression) =>
            new IndexBuilder(
                Builder.HasIndex(
                    Check.NotNull(indexExpression, nameof(indexExpression)).GetPropertyAccessList(),
                    ConfigurationSource.Explicit));

        /// <summary>
        ///     Configures a query used to provide data for a view type.
        /// </summary>
        /// <param name="query"> The query that will provider the underlying data for the view type. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public virtual ViewTypeBuilder<TView> ToQuery([NotNull] Expression<Func<IQueryable<TView>>> query)
        {
            Check.NotNull(query, nameof(query));

            Builder.Metadata[CoreAnnotationNames.DefiningQuery] = query;

            return this;
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this view type has a reference that points
        ///         to a single instance of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see
        ///             cref="ReferenceNavigationBuilder{TView,TRelatedEntity}.WithMany(Expression{Func{TRelatedEntity,IEnumerable{TView}}})" />
        ///         or
        ///         <see
        ///             cref="ReferenceNavigationBuilder{TView,TRelatedEntity}.WithOne(Expression{Func{TRelatedEntity,TView}})" />
        ///         to fully configure the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRelatedEntity"> The view type that this relationship targets. </typeparam>
        /// <param name="navigationExpression">
        ///     A lambda expression representing the reference navigation property on this view type that represents
        ///     the relationship (<c>post => post.Blog</c>). If no property is specified, the relationship will be
        ///     configured without a navigation property on this end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual ReferenceNavigationBuilder<TView, TRelatedEntity> HasOne<TRelatedEntity>(
            [CanBeNull] Expression<Func<TView, TRelatedEntity>> navigationExpression = null)
            where TRelatedEntity : class
        {
            var relatedEntityType = Builder.Metadata.FindInDefinitionPath(typeof(TRelatedEntity)) ??
                                    Builder.ModelBuilder.Entity(typeof(TRelatedEntity), ConfigurationSource.Explicit)
                                        .Metadata;
            var navigation = navigationExpression?.GetPropertyAccess();

            return new ReferenceNavigationBuilder<TView, TRelatedEntity>(
                Builder.Metadata,
                relatedEntityType,
                navigation,
                Builder.Navigation(
                    relatedEntityType.Builder, navigation, ConfigurationSource.Explicit,
                    Builder.Metadata == relatedEntityType));
        }

        /// <summary>
        ///     <para>
        ///         Sets the <see cref="PropertyAccessMode" /> to use for all properties of this view type.
        ///     </para>
        ///     <para>
        ///         By default, the backing field, if one is found by convention or has been specified, is used when
        ///         new objects are constructed, typically when entities are queried from the database.
        ///         Properties are used for all other accesses.  Calling this method witll change that behavior
        ///         for all properties of this view type as described in the <see cref="PropertyAccessMode" /> enum.
        ///     </para>
        ///     <para>
        ///         Calling this method overrrides for all properties of this view type any access mode that was
        ///         set on the model.
        ///     </para>
        /// </summary>
        /// <param name="propertyAccessMode"> The <see cref="PropertyAccessMode" /> to use for properties of this view type. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public new virtual ViewTypeBuilder<TView> UsePropertyAccessMode(PropertyAccessMode propertyAccessMode) =>
            (ViewTypeBuilder<TView>)base.UsePropertyAccessMode(propertyAccessMode);
    }
}
