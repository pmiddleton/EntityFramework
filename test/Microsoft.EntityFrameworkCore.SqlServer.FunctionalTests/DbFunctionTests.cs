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

        #region Table Valued Tests

        [Fact]
        public void TV_Function_Stand_Alone()
        {
            using (var context = CreateContext())
            {
                var reports = (from r in context.FindReportsForManager()
                               select r).ToList();

                Assert.Equal(9, reports.Count);
                Assert.Equal(5, reports.Where(r => r.ManagerId == 2).Count());
                Assert.Equal(3, reports.Where(r => r.ManagerId == 5).Count());
                Assert.Equal(1, reports.Where(r => r.ManagerId == null).Count());
            }
        }

        [Fact]
        public void TV_Function_Stand_Alone_Parameter()
        {
            using (var context = CreateContext())
            {
                var reports = (from r in context.GetEmployeeOrderCountByYear(5)
                               orderby r.OrderCount descending
                               select r).ToList();

                Assert.Equal(3, reports.Count);
                Assert.Equal(18, reports[0].OrderCount);
                Assert.Equal(13, reports[1].OrderCount);
                Assert.Equal(11, reports[2].OrderCount);
            }
        }

        [Fact]
        public void TV_Function_Stand_Alone_Nested()
        {
            using (var context = CreateContext())
            {
                var reports = (from r in context.GetEmployeeOrderCountByYear(() => context.AddValues(2, 3))
                               orderby r.OrderCount descending
                               select r).ToList();

                Assert.Equal(3, reports.Count);
                Assert.Equal(18, reports[0].OrderCount);
                Assert.Equal(13, reports[1].OrderCount);
                Assert.Equal(11, reports[2].OrderCount);
            }
        }

        [Fact]
        public void TV_Function_CrossApply_Correlated_Select_Anonymous()
        {
            using (var context = CreateContext())
            {
                var orders = (from e in context.Employees
                              from r in context.GetEmployeeOrderCountByYear(e.EmployeeID)
                              orderby r.Year, e.EmployeeID
                              select new
                              {
                                  e.EmployeeID,
                                  e.LastName,
                                  r.Year,
                                  r.OrderCount
                              }).ToList();

                Assert.Equal(27, orders.Count);
                Assert.Equal(26, orders[0].OrderCount);
                Assert.Equal(16, orders[1].OrderCount);
                Assert.Equal(18, orders[2].OrderCount);
            }
        }

        [Fact]
        public void TV_Function_CrossApply_Correlated_Select_Result()
        {
            using (var context = CreateContext())
            {
                var orders = (from e in context.Employees
                              from r in context.GetEmployeeOrderCountByYear(e.EmployeeID)
                              orderby r.OrderCount descending
                              select r).ToList();

                Assert.Equal(27, orders.Count);
                Assert.Equal(81, orders[0].OrderCount);
                Assert.Equal(71, orders[1].OrderCount);
                Assert.Equal(55, orders[2].OrderCount);
            }
        }

        [Fact]
        public void TV_Function_CrossJoin_Not_Correlated()
        {
            //TODO - this is selecting too many employee columns in the query
            using (var context = CreateContext())
            {
                var orders = (from e in context.Employees
                              from r in context.GetEmployeeOrderCountByYear(5)
                              where e.EmployeeID == 5
                              orderby r.Year
                              select new
                              {
                                  e.EmployeeID,
                                  e.LastName,
                                  r.Year,
                                  r.OrderCount
                              }).ToList();

                Assert.Equal(3, orders.Count);
                Assert.Equal(11, orders[0].OrderCount);
                Assert.Equal(18, orders[1].OrderCount);
                Assert.Equal(13, orders[2].OrderCount);
            }
        }

        [Fact]
        public void TV_Function_CrossJoin_Parameter()
        {
            //TODO - this is selecting too many employee columns in the query
            using (var context = CreateContext())
            {
                int employeeId = 5;

                var orders = (from e in context.Employees
                              from r in context.GetEmployeeOrderCountByYear(employeeId)
                              where e.EmployeeID == employeeId
                              orderby r.Year
                              select new
                              {
                                  e.EmployeeID,
                                  e.LastName,
                                  r.Year,
                                  r.OrderCount
                              }).ToList();

                Assert.Equal(3, orders.Count);
                Assert.Equal(11, orders[0].OrderCount);
                Assert.Equal(18, orders[1].OrderCount);
                Assert.Equal(13, orders[2].OrderCount);
            }
        }

        [Fact]
        public void TV_Function_Join()
        {
            //performing a join requires the method to have a body which calls ExecuteTableValuedFunction
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopThreeSellingProducts() on p.ProductID equals r.ProductId
                                select new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    r.AmountSold
                                }).ToList();

                Assert.Equal(3, products.Count);
                Assert.Equal(1577, products[0].AmountSold);
                Assert.Equal(1496, products[1].AmountSold);
                Assert.Equal(1397, products[2].AmountSold);
                Assert.Equal("Camembert Pierrot", products[0].ProductName);
                Assert.Equal("Raclette Courdavault", products[1].ProductName);
                Assert.Equal("Gorgonzola Telino", products[2].ProductName);
            }
        }

        [Fact]
        public void TV_Function_LeftJoin_Select_Anonymous()
        {
            //performing a join requires the method to have a body which calls ExecuteTableValuedFunction
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopThreeSellingProducts() on p.ProductID equals r.ProductId into joinTable
                                from j in joinTable.DefaultIfEmpty()
                                orderby p.ProductID descending
                                select new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    j.AmountSold
                                }).ToList();

                Assert.Equal(77, products.Count);
                Assert.Equal(null, products[7].AmountSold);
                Assert.Equal(1577, products[17].AmountSold);
                Assert.Equal(1496, products[18].AmountSold);
                Assert.Equal(1397, products[46].AmountSold);
                Assert.Equal("Outback Lager", products[7].ProductName);
                Assert.Equal("Camembert Pierrot", products[17].ProductName);
                Assert.Equal("Raclette Courdavault", products[18].ProductName);
                Assert.Equal("Gorgonzola Telino", products[46].ProductName);
            }
        }

        [Fact]
        public void TV_Function_LeftJoin_Select_Result()
        {
            //TODO - currently fails because EF tries to change track the result item "o" which is null due to the outer apply
            //resolve once we figure out what kind of Type TVF return types are

            //performing a join requires the method to have a body which calls ExecuteTableValuedFunction
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopThreeSellingProducts() on p.ProductID equals r.ProductId into joinTable
                                from j in joinTable.DefaultIfEmpty()
                                orderby p.ProductID descending
                                select j).ToList();

                Assert.Equal(77, products.Count);
                Assert.Equal(null, products[7].AmountSold);
                Assert.Equal(1577, products[17].AmountSold);
                Assert.Equal(1496, products[18].AmountSold);
                Assert.Equal(1397, products[46].AmountSold);
                //Assert.Equal("Outback Lager", products[7].ProductName);
                //Assert.Equal("Camembert Pierrot", products[17].ProductName);
                //Assert.Equal("Raclette Courdavault", products[18].ProductName);
                //Assert.Equal("Gorgonzola Telino", products[46].ProductName);
            }
        }

        [Fact]
        public void TV_Function_Nested()
        {
            //TODO - this is selecting too many employee columns in the query
            using (var context = CreateContext())
            {
                int employeeId = 5;

                var orders = (from e in context.Employees
                              from r in context.GetEmployeeOrderCountByYear(context.AddValues(2, 3))
                              where e.EmployeeID == employeeId
                              orderby r.Year
                              select new
                              {
                                  e.EmployeeID,
                                  e.LastName,
                                  r.Year,
                                  r.OrderCount
                              }).ToList();

                Assert.Equal(3, orders.Count);
                Assert.Equal(11, orders[0].OrderCount);
                Assert.Equal(18, orders[1].OrderCount);
                Assert.Equal(13, orders[2].OrderCount);
            }
        }
        #endregion

        [Fact]
        public void TV_Function_Join_Nested()
        {
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopThreeSellingProductsForYear(() => context.GetBestYearEver()) on p.ProductID equals r.ProductId
                                select new
                                {
                                    p.ProductID,
                                    p.ProductName,
                                    r.AmountSold
                                }).ToList();

                Assert.Equal(3, products.Count);
                Assert.Equal(659, products[0].AmountSold);
                Assert.Equal(546, products[1].AmountSold);
                Assert.Equal(542, products[2].AmountSold);
                Assert.Equal("Konbu", products[0].ProductName);
                Assert.Equal("Guaraná Fantástica", products[1].ProductName);
                Assert.Equal("Camembert Pierrot", products[2].ProductName);
            }
        }

        //TODO - test that throw exceptions when parameter type mismatch between c# definition and sql function (wrong names, types (when not convertable) etc)

        [Fact]
        public void TV_Function_OuterApply_Correlated_Select_Anonymous()
        {
            using (var context = CreateContext())
            {
                var orders = (from c in context.Customers
                              from o in context.GetLatestNOrdersForCustomer(2, c.CustomerID).DefaultIfEmpty()
                              select new
                              {
                                  c.CustomerID,
                                  o.OrderId,
                                  o.OrderDate
                              }).ToList();

                Assert.Equal(179, orders.Count);
            }
        }

        [Fact]
        public void CrossJoin()
        {
            using (var context = CreateContext())
            {
                var foo = (from c in context.Customers
                               //from p in context.Products
                               //select new { c, p }).ToList();
                           select c).ToList();
            }
        }

        [Fact]
        public void TV_Function_OuterApply_Correlated_Select_Result()
        {
            //TODO - currently fails because EF tries to change track the result item "o" which is null due to the outer apply
            //resolve once we figure out what kind of Type TVF return types are
            using (var context = CreateContext())
            {
                var orders = (from c in context.Customers
                              where c.CustomerID == "FISSA" || c.CustomerID == "BOLID"
                              from o in context.GetLatestNOrdersForCustomer(2, c.CustomerID).DefaultIfEmpty()
                              select new { c, o }).ToList();

                Assert.Equal(3, orders.Count);
            }
        }

        /* [Fact]
         public void LeftOuterJoin()
         {
             //TODO - currently fails because EF tries to change track the result item "o" which is null due to the outer apply
             //resolve once we figure out what kind of Type TVF return types are
             using (var context = CreateContext())
             {
                 var orders = (from c in context.Customers
                               where c.CustomerID == "FISSA" || c.CustomerID == "BOLID"
                               join o in context.Orders on c.CustomerID equals o.CustomerID into temp
                               from t in temp.DefaultIfEmpty()
                               select t).ToList();

                 Assert.Equal(832, orders.Count);
             }
         }*/
    }
}


