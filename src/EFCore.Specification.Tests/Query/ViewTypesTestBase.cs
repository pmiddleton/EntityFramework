// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestModels;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;

// ReSharper disable InconsistentNaming

namespace Microsoft.EntityFrameworkCore.Query
{
    // ReSharper disable once UnusedTypeParameter
    public abstract class ViewTypesTestBase<TFixture>
        where TFixture : ViewTypesFixtureBase, new()
    {
        // TODO:
        // - Inheritance, all view types in hierarchy
        // - DbView props on context?
        // - Mixed tracking?
        // - Conventions - can't be principal, key detection?
        // - State manager
        // - Migrations ignores
        // - Query filters
        // - Defining query parameterization
        // - ToQuery cannot have include/navs?
        // - Async
        // - Mixed ETs and VTs via anonymous projection
        // - Combining ToTable and ToQuery

        [ConditionalFact]
        public virtual void View_simple()
        {
            using (var context = CreateContext())
            {

            }
        }

        protected ViewTypesTestBase(TFixture fixture) => Fixture = fixture;

        protected TFixture Fixture { get; }

        protected ViewTypesContext CreateContext() => Fixture.CreateContext();
    }
}
