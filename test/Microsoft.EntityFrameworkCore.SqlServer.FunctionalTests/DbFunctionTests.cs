// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests.TestModels;
using Xunit;

namespace Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests
{
    public class DbFunctionTests : IClassFixture<NorthwindDbFunctionSqlServerFixture>
    {
        private readonly NorthwindQuerySqlServerFixture _fixture;

        public DbFunctionTests(NorthwindDbFunctionSqlServerFixture fixture)
        {
            _fixture = fixture;
        }

        protected SqlServerDbFunctionsNorthwindContext CreateContext() => _fixture.CreateContext() as SqlServerDbFunctionsNorthwindContext;

        #region Scalar Tests

        private static int AddFive(int number)
        {
            return number + 5;
        }

        [Fact]
        public void Scalar_Function_ClientEval_Method__As_Translateable_Method_Parameter()
        {
            using (var context = CreateContext())
            {
                Assert.Throws<NotImplementedException>(() => (from e in context.Employees
                                                              where e.EmployeeID == 5
                                                              select new
                                                              {
                                                                  FirstName = e.FirstName,
                                                                  OrderCount = context.EmployeeOrderCount(DbFunctionTests.AddFive(e.EmployeeID - 5))
                                                              }).Single());
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Correlated()
        {
            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           where e.EmployeeID == 5
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = context.EmployeeOrderCount(e.EmployeeID)
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal(42, emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Not_Correlated()
        {
            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           where e.EmployeeID == 5
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = context.EmployeeOrderCount(5)
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal(42, emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Parameter()
        {
            using (var context = CreateContext())
            {
                int employeeId = 5;

                var emp = (from e in context.Employees
                           where e.EmployeeID == employeeId
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = context.EmployeeOrderCount(employeeId)
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal(42, emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Nested()
        {
            using (var context = CreateContext())
            {
                int employeeId = 5;
                int starCount = 3;

                var emp = (from e in context.Employees
                           where e.EmployeeID == employeeId
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = context.StarValue(starCount, context.EmployeeOrderCount(employeeId))
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal("***42", emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Where_Correlated()
        {
            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           where context.IsTopEmployee(e.EmployeeID)
                           select e.EmployeeID.ToString().ToLower()).ToList();

                Assert.Equal(3, emp.Count);
                //     Assert.True(emp.Contains(4));
                //   Assert.True(emp.Contains(5));
                // Assert.True(emp.Contains(8));
            }
        }

        [Fact]
        public void Scalar_Function_Where_Not_Correlated()
        {
            using (var context = CreateContext())
            {
                DateTime startDate = DateTime.Parse("1/1/1998");

                var emp = (from e in context.Employees
                           where context.GetEmployeeWithMostOrdersAfterDate(startDate) == e.EmployeeID
                           select e).SingleOrDefault();

                Assert.NotNull(emp);
                Assert.True(emp.EmployeeID == 4);
            }
        }

        [Fact]
        public void Scalar_Function_Where_Parameter()
        {
            using (var context = CreateContext())
            {
                var period = SqlServerDbFunctionsNorthwindContext.ReportingPeriod.Winter;

                var emp = (from e in context.Employees
                           where e.EmployeeID == context.GetEmployeeWithMostOrdersAfterDate(
                                                    context.GetReportingPeriodStartDate(period))
                           select e).SingleOrDefault();

                Assert.NotNull(emp);
                Assert.True(emp.EmployeeID == 4);
            }
        }

        [Fact]
        public void Scalar_Function_Where_Nested()
        {
            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           where e.EmployeeID == context.GetEmployeeWithMostOrdersAfterDate(
                                                   context.GetReportingPeriodStartDate(
                                                       SqlServerDbFunctionsNorthwindContext.ReportingPeriod.Winter))
                           select e).SingleOrDefault();

                Assert.NotNull(emp);
                Assert.True(emp.EmployeeID == 4);
            }
        }

        [Fact]
        public void Scalar_Function_Let_Correlated()
        {
            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           let orderCount = context.EmployeeOrderCount(e.EmployeeID)
                           where e.EmployeeID == 5
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = orderCount
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal(42, emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Let_Not_Correlated()
        {
            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           let orderCount = context.EmployeeOrderCount(5)
                           where e.EmployeeID == 5
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = orderCount
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal(42, emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Let_Not_Parameter()
        {
            var employeeId = 5;

            using (var context = CreateContext())
            {
                var emp = (from e in context.Employees
                           let orderCount = context.EmployeeOrderCount(employeeId)
                           where e.EmployeeID == employeeId
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = orderCount
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal(42, emp.OrderCount);
            }
        }

        [Fact]
        public void Scalar_Function_Let_Nested()
        {
            using (var context = CreateContext())
            {
                int employeeId = 5;
                int starCount = 3;

                var emp = (from e in context.Employees
                           let orderCount = context.StarValue(starCount, context.EmployeeOrderCount(employeeId))
                           where e.EmployeeID == employeeId
                           select new
                           {
                               FirstName = e.FirstName,
                               OrderCount = orderCount
                           }).Single();

                Assert.Equal("Steven", emp.FirstName);
                Assert.Equal("***42", emp.OrderCount);
            }
        }

        #endregion

    }
}


