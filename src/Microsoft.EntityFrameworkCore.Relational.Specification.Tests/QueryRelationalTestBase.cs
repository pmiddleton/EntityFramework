// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Specification.Tests;
using Microsoft.EntityFrameworkCore.Specification.Tests.TestUtilities.Xunit;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Relational.Specification.Tests
{
    public abstract class QueryRelationalTestBase<TFixture> :  QueryTestBase<TFixture>
         where TFixture : NorthwindQueryFixtureBase, new()
    {
        protected QueryRelationalTestBase(TFixture fixture) : base(fixture)
        {
        }

        [ConditionalFact]
        public virtual void DbFunction_Left()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where EF.Functions.Left(c.City, 3) == "Sea"
                               select new { c.CustomerID }).ToList();

                Assert.Equal(1, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_Right()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where EF.Functions.Right(c.City, 2) == "le"
                               select new { c.CustomerID }).ToList();

                Assert.Equal(3, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_Reverse()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where EF.Functions.Reverse(c.City) == "elttaeS"
                               select new { c.CustomerID }).ToList();

                Assert.Equal(1, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_Truncate()
        {
            using (var context = CreateContext())
            {
                var results = (context.OrderDetails.Where(od => EF.Functions.Truncate(od.UnitPrice, 0) > 10)).ToList();

                Assert.Equal(1658, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddYears_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddYears(o.OrderDate, 3).Value.Year == 1999)).ToList();

                Assert.Equal(152, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddMonths_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddMonths(o.OrderDate, 2).Value.Month == 5)).ToList();

                Assert.Equal(103, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddDays_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddDays(o.OrderDate, 2).Value.Day == 5)).ToList();

                Assert.Equal(30, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddHours_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddHours(o.OrderDate, 11).Value.Hour == 11)).ToList();

                Assert.Equal(830, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddMinutes_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddMinutes(o.OrderDate, 7).Value.Minute == 7)).ToList();

                Assert.Equal(830, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddSeconds_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddSeconds(o.OrderDate, 6).Value.Second == 6)).ToList();

                //Assert.Equal(830, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_AddMilliSeconds_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.AddMilliseconds(o.OrderDate, 843).Value.Millisecond == 843)).ToList();

                Assert.Equal(830, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffYears_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffYears(o.OrderDate, DateTime.Parse("1/1/2016")) == 20)).ToList();

                Assert.Equal(152, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffMonths_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffMonths(o.OrderDate, DateTime.Parse("1996-09-22")) == -4)).ToList();

                Assert.Equal(33, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffDays_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffDays(o.OrderDate, DateTime.Parse("1996-10-22")) == -7)).ToList();

                Assert.Equal(2, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffHours_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffHours(o.OrderDate, DateTime.Parse("1996-10-22 00:00:00")) > 0)).ToList();

                Assert.Equal(87, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffMinutes_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffMinutes(o.OrderDate, DateTime.Parse("1996-10-22 00:00:00")) > 0)).ToList();

                Assert.Equal(87, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffSeconds_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffSeconds(o.OrderDate, DateTime.Parse("1996-10-22")) > 0)).ToList();

                Assert.Equal(87, results.Count);
            }
        }

        [ConditionalFact]
        public virtual void DbFunction_DiffMilliSeconds_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.DiffMilliseconds(o.OrderDate, DateTime.Parse("1996-10-22")) < 0
                                    && o.OrderID == 10336)).ToList();

                Assert.Equal(1, results.Count);
            }
        }


        [ConditionalFact]
        public virtual void DbFunction_TruncateTime_Date()
        {
            using (var context = CreateContext())
            {
                var results = (context.Orders.Where(o => EF.Functions.TruncateTime(o.OrderDate) == EF.Functions.TruncateTime(DateTime.Parse("1996-10-22 1:23:45")))).ToList();

                Assert.Equal(1, results.Count);
            }
        }

    }
}
