// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;


namespace Microsoft.EntityFrameworkCore.Infrastructure.Tests
{
    public class CoreModelValidatorTest : ModelValidatorTest
    {
        [Fact]
        public virtual void Detects_shadow_entities()
        {
            var model = new Model();
            model.AddEntityType("A");

            VerifyError(CoreStrings.ShadowEntity("A"), model);
        }

        [Fact]
        public virtual void Detects_shadow_keys()
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityType);
            var keyProperty = entityType.AddProperty("Key", typeof(int));
            entityType.AddKey(keyProperty);

            VerifyWarning(CoreStrings.ShadowKey("{'Key'}", typeof(A).Name, "{'Key'}"), model);
        }

        [Fact]
        public virtual void Detects_shadow_keys_by_convention()
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityType);
            var keyProperty = entityType.AddProperty("Key", typeof(int), configurationSource: ConfigurationSource.Convention);
            entityType.AddKey(keyProperty);

            VerifyWarning(CoreStrings.ShadowKey("{'Key'}", typeof(A).Name, "{'Key'}"), model);
        }

        [Fact]
        public virtual void Detects_shadow_key_referenced_by_foreign_key()
        {
            var modelBuilder = new InternalModelBuilder(new Model());
            var dependentEntityBuilder = modelBuilder.Entity(typeof(SampleEntity), ConfigurationSource.Convention);
            dependentEntityBuilder.Property("Id", typeof(int), ConfigurationSource.Convention);
            dependentEntityBuilder.PrimaryKey(new List<string> { "Id" }, ConfigurationSource.Convention);
            var principalEntityBuilder = modelBuilder.Entity(typeof(ReferencedEntity), ConfigurationSource.Convention);
            principalEntityBuilder.Property("Id", typeof(int), ConfigurationSource.Convention);
            principalEntityBuilder.PrimaryKey(new List<string> { "Id" }, ConfigurationSource.Convention);

            dependentEntityBuilder.Property("Foo", typeof(string), ConfigurationSource.Convention);
            principalEntityBuilder.Property("ReferencedFoo", typeof(string), ConfigurationSource.Convention);
            dependentEntityBuilder.HasForeignKey(
                principalEntityBuilder,
                dependentEntityBuilder.GetOrCreateProperties(new List<string> { "Foo" }, ConfigurationSource.Convention),
                principalEntityBuilder.HasKey(new[] { "ReferencedFoo" }, ConfigurationSource.Convention).Metadata,
                ConfigurationSource.Explicit);

            VerifyWarning(CoreStrings.ShadowKey("{'ReferencedFoo'}", typeof(ReferencedEntity).Name, "{'ReferencedFoo'}"), modelBuilder.Metadata);
        }

        [Fact]
        public virtual void Detects_shadow_key_referenced_by_foreign_key_by_convention()
        {
            var modelBuilder = new InternalModelBuilder(new Model());
            var dependentEntityBuilder = modelBuilder.Entity(typeof(SampleEntity), ConfigurationSource.Convention);
            dependentEntityBuilder.Property("Id", typeof(int), ConfigurationSource.Convention);
            dependentEntityBuilder.PrimaryKey(new List<string> { "Id" }, ConfigurationSource.Convention);
            var principalEntityBuilder = modelBuilder.Entity(typeof(ReferencedEntity), ConfigurationSource.Convention);
            principalEntityBuilder.Property("Id", typeof(int), ConfigurationSource.Convention);
            principalEntityBuilder.PrimaryKey(new List<string> { "Id" }, ConfigurationSource.Convention);

            dependentEntityBuilder.Property("Foo", typeof(string), ConfigurationSource.Convention);
            principalEntityBuilder.Property("ReferencedFoo", typeof(string), ConfigurationSource.Convention);
            dependentEntityBuilder.HasForeignKey(
                principalEntityBuilder,
                dependentEntityBuilder.GetOrCreateProperties(new List<string> { "Foo" }, ConfigurationSource.Convention),
                principalEntityBuilder.HasKey(new[] { "ReferencedFoo" }, ConfigurationSource.Convention).Metadata,
                ConfigurationSource.Convention);

            VerifyError(CoreStrings.ReferencedShadowKey(
                typeof(SampleEntity).Name,
                typeof(ReferencedEntity).Name,
                "{'Foo' : string}",
                "{'Id' : int}"),
                modelBuilder.Metadata);
        }

        [Fact]
        public virtual void Detects_a_null_primary_key()
        {
            var model = new Model();
            model.AddEntityType(typeof(A));

            VerifyError(CoreStrings.EntityRequiresKey(nameof(A)), model);
        }

        [Fact]
        public virtual void Passes_on_escapable_foreign_key_cycles()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var keyA1 = CreateKey(entityA);
            var keyA2 = CreateKey(entityA, startingPropertyIndex: 0, propertyCount: 2);
            var entityB = model.AddEntityType(typeof(B));
            SetPrimaryKey(entityB);
            var keyB1 = CreateKey(entityB);
            var keyB2 = CreateKey(entityB, startingPropertyIndex: 1, propertyCount: 2);

            CreateForeignKey(keyA1, keyB1);
            CreateForeignKey(keyB1, keyA1);
            CreateForeignKey(keyA2, keyB2);

            Validate(model);
        }

        [Fact]
        public virtual void Passes_on_escapable_foreign_key_cycles_not_starting_at_hub()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var keyA1 = CreateKey(entityA);
            var keyA2 = CreateKey(entityA, startingPropertyIndex: 1, propertyCount: 2);
            var entityB = model.AddEntityType(typeof(B));
            SetPrimaryKey(entityB);
            var keyB1 = CreateKey(entityB);
            var keyB2 = CreateKey(entityB, startingPropertyIndex: 0, propertyCount: 2);

            CreateForeignKey(keyA1, keyB1);
            CreateForeignKey(keyB1, keyA1);
            CreateForeignKey(keyB2, keyA2);

            Validate(model);
        }

        [Fact]
        public virtual void Passes_on_foreign_key_cycle_with_one_GenerateOnAdd()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var keyA = CreateKey(entityA);
            var entityB = model.AddEntityType(typeof(B));
            SetPrimaryKey(entityB);
            var keyB = CreateKey(entityB);

            CreateForeignKey(keyA, keyB);
            CreateForeignKey(keyB, keyA);

            keyA.Properties[0].ValueGenerated = ValueGenerated.OnAdd;

            Validate(model);
        }

        [Fact]
        public virtual void Pases_on_double_reference_to_root_principal_property()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var keyA1 = CreateKey(entityA);
            var keyA2 = CreateKey(entityA, startingPropertyIndex: 0, propertyCount: 2);
            var entityB = model.AddEntityType(typeof(B));
            SetPrimaryKey(entityB);
            var keyB1 = CreateKey(entityB);
            var keyB2 = CreateKey(entityB, startingPropertyIndex: 0, propertyCount: 2);

            CreateForeignKey(keyA1, keyB1);
            CreateForeignKey(keyA2, keyB2);

            Validate(model);
        }

        [Fact]
        public virtual void Pases_on_diamond_path_to_root_principal_property()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var keyA1 = CreateKey(entityA);
            var keyA2 = CreateKey(entityA, startingPropertyIndex: 0, propertyCount: 2);
            var keyA3 = CreateKey(entityA);
            var keyA4 = CreateKey(entityA, startingPropertyIndex: 2, propertyCount: 2);
            var entityB = model.AddEntityType(typeof(B));
            SetPrimaryKey(entityB);
            var keyB1 = CreateKey(entityB);
            var keyB2 = CreateKey(entityB, startingPropertyIndex: 1, propertyCount: 2);

            CreateForeignKey(keyA1, keyB1);
            CreateForeignKey(keyA2, keyB2);

            CreateForeignKey(keyB1, keyA3);
            CreateForeignKey(keyB2, keyA4);

            Validate(model);
        }

        [Fact]
        public virtual void Pases_on_correct_inheritance()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var entityD = model.AddEntityType(typeof(D));
            SetBaseType(entityD, entityA);

            Validate(model);
        }

        [Fact]
        public virtual void Detects_base_type_not_set()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var entityD = model.AddEntityType(typeof(D));
            SetPrimaryKey(entityD);

            VerifyError(CoreStrings.InconsistentInheritance(entityD.DisplayName(), entityA.DisplayName()), model);
        }

        [Fact]
        public virtual void Detects_abstract_leaf_type()
        {
            var model = new Model();
            var entityA = model.AddEntityType(typeof(A));
            SetPrimaryKey(entityA);
            var entityAbstract = model.AddEntityType(typeof(Abstract));
            SetBaseType(entityAbstract, entityA);

            VerifyError(CoreStrings.AbstractLeafEntityType(entityAbstract.DisplayName()), model);
        }

        [Fact]
        public virtual void Detects_generic_leaf_type()
        {
            var model = new Model();
            var entityAbstract = model.AddEntityType(typeof(Abstract));
            SetPrimaryKey(entityAbstract);
            var entityGeneric = model.AddEntityType(typeof(Generic<>));
            entityGeneric.HasBaseType(entityAbstract);

            VerifyError(CoreStrings.AbstractLeafEntityType(entityGeneric.DisplayName()), model);
        }

        [Fact]
        public virtual void Pases_on_valid_delegated_identity_entity_types()
        {
            var modelBuilder = new InternalModelBuilder(new Model());
            var entityTypeBuilder = modelBuilder.Entity(typeof(SampleEntity), ConfigurationSource.Convention);
            entityTypeBuilder.PrimaryKey(new[] { nameof(SampleEntity.Id) }, ConfigurationSource.Convention);
            var ownershipBuilder = entityTypeBuilder.Owns(
                typeof(ReferencedEntity), nameof(SampleEntity.ReferencedEntity), ConfigurationSource.Convention);
            var ownedTypeBuilder = ownershipBuilder.Metadata.DeclaringEntityType.Builder;
            ownedTypeBuilder.PrimaryKey(ownershipBuilder.Metadata.Properties.Select(p => p.Name).ToList(), ConfigurationSource.Convention);

            Validate(modelBuilder.Metadata);
        }

        [Fact]
        public virtual void Detects_delegated_identity_entity_type_without_defining_navigation()
        {
            var modelBuilder = new InternalModelBuilder(new Model());
            var entityTypeBuilder = modelBuilder.Entity(typeof(SampleEntity), ConfigurationSource.Convention);
            entityTypeBuilder.PrimaryKey(new[] { nameof(SampleEntity.Id) }, ConfigurationSource.Convention);
            var ownershipBuilder = entityTypeBuilder.Owns(
                typeof(ReferencedEntity), nameof(SampleEntity.ReferencedEntity), ConfigurationSource.Convention);
            var ownedTypeBuilder = ownershipBuilder.Metadata.DeclaringEntityType.Builder;
            ownedTypeBuilder.PrimaryKey(ownershipBuilder.Metadata.Properties.Select(p => p.Name).ToList(), ConfigurationSource.Convention);

            entityTypeBuilder.Metadata.RemoveNavigation(nameof(SampleEntity.ReferencedEntity));

            VerifyError(CoreStrings.NoDefiningNavigation(
                nameof(SampleEntity.ReferencedEntity), nameof(SampleEntity),
                nameof(SampleEntity) + "." + nameof(SampleEntity.ReferencedEntity) + "->" + nameof(ReferencedEntity)),
                modelBuilder.Metadata);
        }

        [Fact]
        public virtual void Detects_delegated_identity_entity_type_with_multiple_ownerships()
        {
            var modelBuilder = new InternalModelBuilder(new Model());
            var entityTypeBuilder = modelBuilder.Entity(typeof(SampleEntity), ConfigurationSource.Convention);
            entityTypeBuilder.PrimaryKey(new[] { nameof(SampleEntity.Id) }, ConfigurationSource.Convention);
            var ownershipBuilder = entityTypeBuilder.Owns(
                typeof(ReferencedEntity), nameof(SampleEntity.ReferencedEntity), ConfigurationSource.Convention);
            var ownedTypeBuilder = ownershipBuilder.Metadata.DeclaringEntityType.Builder;
            ownedTypeBuilder.PrimaryKey(ownershipBuilder.Metadata.Properties.Select(p => p.Name).ToList(), ConfigurationSource.Convention);

            ownedTypeBuilder.Relationship(entityTypeBuilder, (string)null, null, ConfigurationSource.Convention, setTargetAsPrincipal: true)
                .Metadata.IsOwnership = true;

            VerifyError(CoreStrings.MultipleOwnerships(
                nameof(SampleEntity) + "." + nameof(SampleEntity.ReferencedEntity) + "->" + nameof(ReferencedEntity)),
                modelBuilder.Metadata);
        }

        [Fact]
        public virtual void Detects_delegated_identity_entity_type_with_non_defining_ownership()
        {
            var modelBuilder = new InternalModelBuilder(new Model());
            var entityTypeBuilder = modelBuilder.Entity(typeof(SampleEntity), ConfigurationSource.Convention);
            entityTypeBuilder.PrimaryKey(new[] { nameof(SampleEntity.Id) }, ConfigurationSource.Convention);
            var ownershipBuilder = entityTypeBuilder.Owns(
                typeof(ReferencedEntity), nameof(SampleEntity.ReferencedEntity), ConfigurationSource.Convention);
            var ownedTypeBuilder = ownershipBuilder.Metadata.DeclaringEntityType.Builder;
            ownedTypeBuilder.PrimaryKey(ownershipBuilder.Metadata.Properties.Select(p => p.Name).ToList(), ConfigurationSource.Convention);

            ownedTypeBuilder.Relationship(entityTypeBuilder, (string)null, null, ConfigurationSource.Convention, setTargetAsPrincipal: true)
                .IsOwnership(true, ConfigurationSource.Convention);

            VerifyError(CoreStrings.NonDefiningOwnership(
                nameof(SampleEntity),
                nameof(SampleEntity.ReferencedEntity),
                nameof(SampleEntity) + "." + nameof(SampleEntity.ReferencedEntity) + "->" + nameof(ReferencedEntity)),
                modelBuilder.Metadata);
        }

        [Theory]
        [InlineData(ChangeTrackingStrategy.ChangedNotifications)]
        [InlineData(ChangeTrackingStrategy.ChangingAndChangedNotifications)]
        [InlineData(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues)]
        public virtual void Detects_non_notifying_entities(ChangeTrackingStrategy changeTrackingStrategy)
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(NonNotifyingEntity));
            var id = entityType.AddProperty("Id");
            entityType.SetPrimaryKey(id);

            model.ChangeTrackingStrategy = changeTrackingStrategy;

            VerifyError(
                CoreStrings.ChangeTrackingInterfaceMissing("NonNotifyingEntity", changeTrackingStrategy, "INotifyPropertyChanged"),
                model);
        }

        [Theory]
        [InlineData(ChangeTrackingStrategy.ChangingAndChangedNotifications)]
        [InlineData(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues)]
        public virtual void Detects_changed_only_notifying_entities(ChangeTrackingStrategy changeTrackingStrategy)
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(ChangedOnlyEntity));
            var id = entityType.AddProperty("Id");
            entityType.SetPrimaryKey(id);

            model.ChangeTrackingStrategy = changeTrackingStrategy;

            VerifyError(
                CoreStrings.ChangeTrackingInterfaceMissing("ChangedOnlyEntity", changeTrackingStrategy, "INotifyPropertyChanging"),
                model);
        }

        [Theory]
        [InlineData(ChangeTrackingStrategy.Snapshot)]
        [InlineData(ChangeTrackingStrategy.ChangedNotifications)]
        [InlineData(ChangeTrackingStrategy.ChangingAndChangedNotifications)]
        [InlineData(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues)]
        public virtual void Passes_for_fully_notifying_entities(ChangeTrackingStrategy changeTrackingStrategy)
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(FullNotificationEntity));
            var id = entityType.AddProperty("Id");
            entityType.SetPrimaryKey(id);

            model.ChangeTrackingStrategy = changeTrackingStrategy;

            Validate(model);
        }

        [Theory]
        [InlineData(ChangeTrackingStrategy.Snapshot)]
        [InlineData(ChangeTrackingStrategy.ChangedNotifications)]
        public virtual void Passes_for_changed_only_entities_with_snapshot_or_changed_only_tracking(ChangeTrackingStrategy changeTrackingStrategy)
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(ChangedOnlyEntity));
            var id = entityType.AddProperty("Id");
            entityType.SetPrimaryKey(id);

            model.ChangeTrackingStrategy = changeTrackingStrategy;

            Validate(model);
        }

        [Fact]
        public virtual void Passes_for_non_notifying_entities_with_snapshot_tracking()
        {
            var model = new Model();
            var entityType = model.AddEntityType(typeof(NonNotifyingEntity));
            var id = entityType.AddProperty("Id");
            entityType.SetPrimaryKey(id);

            model.ChangeTrackingStrategy = ChangeTrackingStrategy.Snapshot;

            Validate(model);
        }


        [Fact]
        public virtual void Detects_missing_function_name()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetMethods().Where(m => m.Name == nameof(string.ToLower) && m.GetParameters().Count() == 0).Single());

            dbFunc.Name = null;

            VerifyError(CoreStrings.DbFunctionNameEmpty(), model);
        }

        [Fact]
        public virtual void Detects_function_duplicate_parameter_index()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetMethods().Where(m => m.Name == nameof(string.ToLower) && m.GetParameters().Count() == 0).Single());

            dbFunc.AddParameter("a").SetParameterIndex(0);
            dbFunc.AddParameter("b").SetParameterIndex(0);

            VerifyError(CoreStrings.DbFunctionDuplicateIndex("String.ToLower"), model);
        }

        [Fact]
        public virtual void Detects_function_missing_parameter_index()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetMethods().Where(m => m.Name == nameof(string.ToLower) && m.GetParameters().Count() == 0).Single());

            dbFunc.AddParameter("a").SetParameterIndex(1);

            VerifyError(CoreStrings.DbFunctionNonContinuousIndex("String.ToLower"), model);
        }

        [Fact]
        public virtual void Detects_function_with_non_constant_identifier_parameter()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetMethods().Where(m => m.Name == nameof(string.ToLower) && m.GetParameters().Count() == 0).Single());

            var dbParam = dbFunc.AddParameter("a"); ;
            dbParam.SetParameterIndex(0);
            dbParam.IsIdentifier = true;

            VerifyError(CoreStrings.DbFunctionIdentifierMustBeCompileTimeConstant("String.ToLower", dbParam.Name), model);
        }

        [Fact]
        public virtual void Detects_function_object_parameter_on_static_method()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetDeclaredMethod(nameof(string.IsNullOrEmpty)));

            var dbParam = dbFunc.AddParameter("a"); ;
            dbParam.SetParameterIndex(0);
            dbParam.IsObjectParameter = true;

            VerifyError(CoreStrings.DbFunctionObjectParameterOnStaticMethod("String.IsNullOrEmpty", dbParam.Name), model);
        }

        [Fact]
        public virtual void Detects_function_with_array_parameter_but_no_translate_callback()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetDeclaredMethod(nameof(string.IsNullOrEmpty)));

            var dbParam = dbFunc.AddParameter("a"); ;
            dbParam.SetParameterIndex(0);
            dbParam.ParameterType = typeof(string[]);

            VerifyError(CoreStrings.DbFunctionParameterArrayNoTranslate("String.IsNullOrEmpty", dbParam.Name), model);
        }

        [Fact]
        public virtual void Detects_function_with_parameter_with_missing_type()
        {
            var model = new Model();

            var dbFunc = model.AddDbFunction(typeof(string).GetTypeInfo().GetDeclaredMethod(nameof(string.IsNullOrEmpty)));

            var dbParam = dbFunc.AddParameter("a"); ;
            dbParam.IsObjectParameter = true;

            VerifyError(CoreStrings.DbFunctionObjectParameterOnStaticMethod("String.IsNullOrEmpty", dbParam.Name), model);
        }

        // INotify interfaces not really implemented; just marking the classes to test metadata construction
        private class FullNotificationEntity : INotifyPropertyChanging, INotifyPropertyChanged
        {
            public int Id { get; set; }

#pragma warning disable 67
            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67
        }

        // INotify interfaces not really implemented; just marking the classes to test metadata construction
        private class ChangedOnlyEntity : INotifyPropertyChanged
        {
            public int Id { get; set; }

#pragma warning disable 67
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67
        }

        private class NonNotifyingEntity
        {
            public int Id { get; set; }
        }

        protected override ModelValidator CreateModelValidator()
            => new CoreModelValidator(new ModelValidatorDependencies(Logger));
    }
}
