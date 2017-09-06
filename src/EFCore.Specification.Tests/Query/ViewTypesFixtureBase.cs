// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestModels;

namespace Microsoft.EntityFrameworkCore.Query
{
    public abstract class ViewTypesFixtureBase : SharedStoreFixtureBase<ViewTypesContext>
    {
        protected override string StoreName { get; } = "Northwind";

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
        }
    }
}
