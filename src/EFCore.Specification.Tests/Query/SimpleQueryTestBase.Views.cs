// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Microsoft.EntityFrameworkCore.Query
{
    // ReSharper disable once UnusedTypeParameter
    public abstract partial class SimpleQueryTestBase<TFixture>
    {
        // TODO:
        // - Conventions - can't be principal, key detection?
        // - State manager
        // - Migrations ignores
        // - Query filters
        // - Defining query parameterization
        // - ToQuery cannot have include/navs?
        // - Combining ToTable and ToQuery
        // - Calling Entity after View / View after Entity?
         
        [ConditionalFact]
        public virtual void View_simple()
        {
            AssertQuery<CustomerView>(cvs => cvs);
        }

        [ConditionalFact]
        public virtual void View_where_simple()
        {
            AssertQuery<CustomerView>(
                cvs => cvs.Where(c => c.City == "London"));
        }

        [ConditionalFact]
        public virtual void View_backed_by_view()
        {
            using (var context = CreateContext())
            {
                var results = context.View<ProductView>().ToArray();

                Assert.Equal(69, results.Length);
            }
        }

        [ConditionalFact]
        public virtual void Auto_initialized_view_set()
        {
            using (var context = CreateContext())
            {
                var results = context.CustomerViews.ToArray();

                Assert.Equal(91, results.Length);
            }
        }

        [ConditionalFact]
        public virtual void View_with_nav_defining_query()
        {
            using (var context = CreateContext())
            {
                var results
                    = context.View<CustomerQuery>()
                        .Where(cq => cq.OrderCount > 0)
                        .ToArray();

                Assert.Equal(89, results.Length);
            }
        }

        [ConditionalFact]
        public virtual void View_with_nav()
        {
            AssertQuery<OrderView>(ovs => ovs.Where(ov => ov.CustomerID == "ALFKI"));
        }

        [ConditionalFact]
        public virtual void View_with_mixed_tracking()
        {
            AssertQuery<Customer, OrderView>(
                (cs, ovs)
                    => from c in cs
                       from o in ovs.Where(ov => ov.CustomerID == c.CustomerID)
                       select new
                       {
                           c,
                           o
                       },
                e => e.c.CustomerID,
                entryCount: 89);
        }

        [ConditionalFact]
        public virtual void View_with_included_nav()
        {
            AssertIncludeQuery<OrderView>(
                ovs => from ov in ovs.Include(ov => ov.Customer)
                       where ov.CustomerID == "ALFKI"
                       select ov,
                new List<IExpectedInclude>
                {
                    new ExpectedInclude<OrderView>(ov => ov.Customer, "Customer")
                });
        }

        [ConditionalFact]
        public virtual void View_with_included_navs_multi_level()
        {
            AssertIncludeQuery<OrderView>(
                ovs => from ov in ovs.Include(ov => ov.Customer.Orders)
                       where ov.CustomerID == "ALFKI"
                       select ov,
                new List<IExpectedInclude>
                {
                    new ExpectedInclude<OrderView>(ov => ov.Customer, "Customer"),
                    new ExpectedInclude<Customer>(c => c.Orders, "Orders")
                });
        }

        [ConditionalFact]
        public virtual void View_select_where_navigation()
        {
            AssertQuery<OrderView>(
                ovs => from ov in ovs
                       where ov.Customer.City == "Seattle"
                       select ov);
        }

        [ConditionalFact]
        public virtual void View_select_where_navigation_multi_level()
        {
            AssertQuery<OrderView>(
                ovs => from ov in ovs
                       where ov.Customer.Orders.Any()
                       select ov);
        }
    }
}
