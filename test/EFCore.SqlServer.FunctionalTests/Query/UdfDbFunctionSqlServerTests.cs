// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable InconsistentNaming

namespace Microsoft.EntityFrameworkCore.Query
{
    public class UdfDbFunctionSqlServerTests : IClassFixture<UdfDbFunctionSqlServerTests.SqlServerUDFFixture>
    {
        public UdfDbFunctionSqlServerTests(SqlServerUDFFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        private SqlServerUDFFixture Fixture { get; }

        protected UDFSqlContext CreateContext() => (UDFSqlContext)Fixture.CreateContext();

        public class Customer
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<Order> Orders { get; set; }
        }

        public class Order
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int QuantitySold { get; set; }
            public DateTime OrderDate { get; set; }
            public Customer Customer { get; set; }
            public Product Product { get; set; }
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // ReSharper disable once InconsistentNaming
        protected class UDFSqlContext : DbContext
        {
            #region DbSets

            public DbSet<Customer> Customers { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<Product> Products { get; set; }

            #endregion

            #region Function Stubs

            #region Static Functions

            public enum ReportingPeriod
            {
                Winter = 0,
                Spring,
                Summer,
                Fall
            }

            public static long MyCustomLengthStatic(string s)
            {
                throw new Exception();
            }

            public static bool IsDateStatic(string date)
            {
                throw new Exception();
            }

            public static int AddOneStatic(int num)
            {
                return num + 1;
            }

            public static int AddFiveStatic(int number)
            {
                return number + 5;
            }

            public static int CustomerOrderCountStatic(int customerId)
            {
                throw new NotImplementedException();
            }

            public static int CustomerOrderCountWithClientStatic(int customerId)
            {
                switch (customerId)
                {
                    case 1:
                        return 3;
                    case 2:
                        return 2;
                    case 3:
                        return 1;
                    case 4:
                        return 0;
                    case 5:
                        return 0;
                    default:
                        throw new Exception();
                }
            }

            public static string StarValueStatic(int starCount, int value)
            {
                throw new NotImplementedException();
            }

            public static bool IsTopCustomerStatic(int customerId)
            {
                throw new NotImplementedException();
            }

            public static int GetCustomerWithMostOrdersAfterDateStatic(DateTime? startDate)
            {
                throw new NotImplementedException();
            }

            public static DateTime? GetReportingPeriodStartDateStatic(ReportingPeriod periodId)
            {
                throw new NotImplementedException();
            }

            public long MyCustomLengthInstance(string s)
            {
                throw new Exception();
            }

            public bool IsDateInstance(string date)
            {
                throw new Exception();
            }

            public int AddOneInstance(int num)
            {
                return num + 1;
            }

            public int AddFiveInstance(int number)
            {
                return number + 5;
            }

            public int CustomerOrderCountInstance(int customerId)
            {
                throw new NotImplementedException();
            }

            public int CustomerOrderCountWithClientInstance(int customerId)
            {
                switch (customerId)
                {
                    case 1:
                        return 3;
                    case 2:
                        return 2;
                    case 3:
                        return 1;
                    case 4:
                        return 0;
                    case 5:
                        return 0;
                    default:
                        throw new Exception();
                }
            }

            public string StarValueInstance(int starCount, int value)
            {
                throw new NotImplementedException();
            }

            public bool IsTopCustomerInstance(int customerId)
            {
                throw new NotImplementedException();
            }

            public int GetCustomerWithMostOrdersAfterDateInstance(DateTime? startDate)
            {
                throw new NotImplementedException();
            }

            public DateTime? GetReportingPeriodStartDateInstance(ReportingPeriod periodId)
            {
                throw new NotImplementedException();
            }

            public string DollarValueInstance(int starCount, string value)
            {
                throw new NotImplementedException();
            }

            [DbFunction(Schema = "dbo")]
            public static string IdentityString(string s)
            {
                throw new NotImplementedException();
            }

            public string SCHEMA_NAME()
            {
                //TODO - how to remove the generic params here?
                return ExecuteScalarMethod<UDFSqlContext, string>(db => db.SCHEMA_NAME());
            }

            public int AddValues(int a, int b)
            {
                return ExecuteScalarMethod<UDFSqlContext, int>(db => db.AddValues(a, b));
            }

            public int AddValues(Expression<Func<int>> a, int b)
            {
                return ExecuteScalarMethod<UDFSqlContext, int>(db => db.AddValues(a, b));
            }

            #endregion

            #region Table Functions

            public class OrderByYear
            {
                public int? CustomerId { get; set; }
                public int? Count { get; set; }
                public int? Year { get; set; }
            }

            public IQueryable<OrderByYear> GetCustomerOrderCountByYear(int customerId)
            {
                //   return ExecuteTableValuedFunction<ManagersEmployee>(typeof(SqlServerDbFunctionsNorthwindContext).GetTypeInfo().GetDeclaredMethod(nameof(FindReportsForManager)));

                // return ExecuteTableValuedFunction<OrderByYear>(GetType().GetMethod(nameof(GetCustomerOrderCountByYear)), customerId );
                return ExecuteTableValuedFunction<UDFSqlContext, OrderByYear>(db => db.GetCustomerOrderCountByYear(customerId));
            }

            public IQueryable<OrderByYear> GetCustomerOrderCountByYear(Expression<Func<int>> customerId)
            {
                return ExecuteTableValuedFunction<UDFSqlContext, OrderByYear>(db => db.GetCustomerOrderCountByYear(customerId));
            }

            public class TopSellingProduct
            {
                public int? ProductId { get; set; }
                public int? AmountSold { get; set; }
            }

            public IQueryable<TopSellingProduct> GetTopTwoSellingProducts()
            {
                return ExecuteTableValuedFunction<UDFSqlContext, TopSellingProduct>(db => db.GetTopTwoSellingProducts());
            }

            #endregion

            #endregion

            public UDFSqlContext(DbContextOptions options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //Static
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(CustomerOrderCountStatic))).HasName("CustomerOrderCount");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(CustomerOrderCountWithClientStatic))).HasName("CustomerOrderCount");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(StarValueStatic))).HasName("StarValue");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(IsTopCustomerStatic))).HasName("IsTopCustomer");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerWithMostOrdersAfterDateStatic))).HasName("GetCustomerWithMostOrdersAfterDate");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetReportingPeriodStartDateStatic))).HasName("GetReportingPeriodStartDate");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(IsDateStatic))).HasSchema("").HasName("IsDate");

                var methodInfo = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthStatic));

                modelBuilder.HasDbFunction(methodInfo)
                    .HasTranslation(args => new SqlFunctionExpression("len", methodInfo.ReturnType, args));

                //Instance
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(CustomerOrderCountInstance))).HasName("CustomerOrderCount");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(CustomerOrderCountWithClientInstance))).HasName("CustomerOrderCount");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(StarValueInstance))).HasName("StarValue");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(IsTopCustomerInstance))).HasName("IsTopCustomer");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerWithMostOrdersAfterDateInstance))).HasName("GetCustomerWithMostOrdersAfterDate");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetReportingPeriodStartDateInstance))).HasName("GetReportingPeriodStartDate");
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(IsDateInstance))).HasSchema("").HasName("IsDate");

                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(DollarValueInstance))).HasName("DollarValue");

                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(AddValues), new[] { typeof(int), typeof(int) }));
                //  modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(AddValues), new[] { typeof(Expression<Func<int>>), typeof(int) }));


                var methodInfo2 = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthInstance));

                modelBuilder.HasDbFunction(methodInfo2)
                    .HasTranslation(args => new SqlFunctionExpression("len", methodInfo2.ReturnType, args));

                //Bootstrap
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(SCHEMA_NAME))).HasName("SCHEMA_NAME").HasSchema("");

                //Table
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetCustomerOrderCountByYear), new[] { typeof(int) }));
                modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(GetTopTwoSellingProducts)));
            }
        }

        #region Scalar Tests

        #region Static

        [Fact]
        private void Scalar_Function_Extension_Method_Static()
        {
            using (var context = CreateContext())
            {
                var len = context.Customers.Count(c => UDFSqlContext.IsDateStatic(c.FirstName) == false);

                Assert.Equal(4, len);

                AssertSql(
                    @"SELECT COUNT(*)
FROM [Customers] AS [c]
WHERE CASE
    WHEN IsDate([c].[FirstName]) = 1
    THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
END = 0");
            }
        }

        [Fact]
        private void Scalar_Function_With_Translator_Translates_Static()
        {
            using (var context = CreateContext())
            {
                var customerId = 3;

                var len = context.Customers.Where(c => c.Id == customerId)
                    .Select(c => UDFSqlContext.MyCustomLengthStatic(c.LastName)).Single();

                Assert.Equal(5, len);

                AssertSql(
                    @"@__customerId_0='3'

SELECT TOP(2) len([c].[LastName])
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_0");
            }
        }

#if !Test20
        [Fact]
        public void Scalar_Function_ClientEval_Method_As_Translateable_Method_Parameter_Static()
        {
            using (var context = CreateContext())
            {
                Assert.Throws<NotImplementedException>(
                    () => (from c in context.Customers
                           where c.Id == 1
                           select new
                           {
                               c.FirstName,
                               OrderCount = UDFSqlContext.CustomerOrderCountStatic(UDFSqlContext.AddFiveStatic(c.Id - 5))
                           }).Single());
            }
        }

        [Fact]
        public void Scalar_Function_Constant_Parameter_Static()
        {
            using (var context = CreateContext())
            {
                var customerId = 1;

                var custs = context.Customers.Select(c => UDFSqlContext.CustomerOrderCountStatic(customerId)).ToList();

                Assert.Equal(4, custs.Count);

                AssertSql(
                    @"@__customerId_0='1'

SELECT [dbo].[CustomerOrderCount](@__customerId_0)
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Correlated_Static()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            where c.Id == 1
                            select new
                            {
                                c.LastName,
                                OrderCount = UDFSqlContext.CustomerOrderCountStatic(c.Id)
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal(3, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount]([c].[Id]) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 1");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Not_Correlated_Static()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            where c.Id == 1
                            select new
                            {
                                c.LastName,
                                OrderCount = UDFSqlContext.CustomerOrderCountStatic(1)
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal(3, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](1) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 1");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Parameter_Static()
        {
            using (var context = CreateContext())
            {
                var customerId = 1;

                var cust = (from c in context.Customers
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = UDFSqlContext.CustomerOrderCountStatic(customerId)
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal(3, cust.OrderCount);

                AssertSql(
                    @"@__customerId_1='1'
@__customerId_0='1'

SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](@__customerId_1) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_0");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Nested_Static()
        {
            using (var context = CreateContext())
            {
                var customerId = 3;
                var starCount = 3;

                var cust = (from c in context.Customers
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = UDFSqlContext.StarValueStatic(starCount, UDFSqlContext.CustomerOrderCountStatic(customerId))
                            }).Single();

                Assert.Equal("Three", cust.LastName);
                Assert.Equal("***1", cust.OrderCount);

                AssertSql(
                    @"@__starCount_1='3'
@__customerId_2='3'
@__customerId_0='3'

SELECT TOP(2) [c].[LastName], [dbo].[StarValue](@__starCount_1, [dbo].[CustomerOrderCount](@__customerId_2)) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_0");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Correlated_Static()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            where UDFSqlContext.IsTopCustomerStatic(c.Id)
                            select c.Id.ToString().ToLower()).ToList();

                Assert.Equal(1, cust.Count);

                AssertSql(
                    @"SELECT LOWER(CONVERT(VARCHAR(11), [c].[Id]))
FROM [Customers] AS [c]
WHERE [dbo].[IsTopCustomer]([c].[Id]) = 1");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Not_Correlated_Static()
        {
            using (var context = CreateContext())
            {
                var startDate = new DateTime(2000, 4, 1);

                var custId = (from c in context.Customers
                              where UDFSqlContext.GetCustomerWithMostOrdersAfterDateStatic(startDate) == c.Id
                              select c.Id).SingleOrDefault();

                Assert.Equal(custId, 2);

                AssertSql(
                    @"@__startDate_0='2000-04-01T00:00:00'

SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE [dbo].[GetCustomerWithMostOrdersAfterDate](@__startDate_0) = [c].[Id]");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Parameter_Static()
        {
            using (var context = CreateContext())
            {
                var period = UDFSqlContext.ReportingPeriod.Winter;

                var custId = (from c in context.Customers
                              where c.Id == UDFSqlContext.GetCustomerWithMostOrdersAfterDateStatic(
                                        UDFSqlContext.GetReportingPeriodStartDateStatic(period))
                              select c.Id).SingleOrDefault();

                Assert.Equal(custId, 1);

                AssertSql(
                    @"@__period_0='0'

SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE [c].[Id] = [dbo].[GetCustomerWithMostOrdersAfterDate]([dbo].[GetReportingPeriodStartDate](@__period_0))");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Nested_Static()
        {
            using (var context = CreateContext())
            {
                var custId = (from c in context.Customers
                              where c.Id == UDFSqlContext.GetCustomerWithMostOrdersAfterDateStatic(
                                        UDFSqlContext.GetReportingPeriodStartDateStatic(
                                            UDFSqlContext.ReportingPeriod.Winter))
                              select c.Id).SingleOrDefault();

                Assert.Equal(custId, 1);

                AssertSql(
                    @"SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE [c].[Id] = [dbo].[GetCustomerWithMostOrdersAfterDate]([dbo].[GetReportingPeriodStartDate](0))");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Correlated_Static()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            let orderCount = UDFSqlContext.CustomerOrderCountStatic(c.Id)
                            where c.Id == 2
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("Two", cust.LastName);
                Assert.Equal(2, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount]([c].[Id]) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 2");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Not_Correlated_Static()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            let orderCount = UDFSqlContext.CustomerOrderCountStatic(2)
                            where c.Id == 2
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("Two", cust.LastName);
                Assert.Equal(2, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](2) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 2");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Not_Parameter_Static()
        {
            var customerId = 2;

            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            let orderCount = UDFSqlContext.CustomerOrderCountStatic(customerId)
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("Two", cust.LastName);
                Assert.Equal(2, cust.OrderCount);

                AssertSql(
                    @"@__customerId_0='2'
@__customerId_1='2'

SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](@__customerId_0) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_1");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Nested_Static()
        {
            using (var context = CreateContext())
            {
                var customerId = 1;
                var starCount = 3;

                var cust = (from c in context.Customers
                            let orderCount = UDFSqlContext.StarValueStatic(starCount, UDFSqlContext.CustomerOrderCountStatic(customerId))
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal("***3", cust.OrderCount);

                AssertSql(
                    @"@__starCount_0='3'
@__customerId_1='1'
@__customerId_2='1'

SELECT TOP(2) [c].[LastName], [dbo].[StarValue](@__starCount_0, [dbo].[CustomerOrderCount](@__customerId_1)) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_2");
            }
        }
#endif

        [Fact]
        public void Scalar_Nested_Function_Unwind_Client_Eval_Where_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == UDFSqlContext.AddOneStatic(c.Id)
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Unwind_Client_Eval_OrderBy_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               orderby UDFSqlContext.AddOneStatic(c.Id)
                               select c.Id).ToList();

                Assert.Equal(4, results.Count);
                Assert.True(results.SequenceEqual(Enumerable.Range(1, 4)));

                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Unwind_Client_Eval_Select_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               orderby c.Id
                               select UDFSqlContext.AddOneStatic(c.Id)).ToList();

                Assert.Equal(4, results.Count);
                Assert.True(results.SequenceEqual(Enumerable.Range(2, 4)));

                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]
ORDER BY [c].[Id]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_BCL_UDF_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == UDFSqlContext.AddOneStatic(Math.Abs(UDFSqlContext.CustomerOrderCountWithClientStatic(c.Id)))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_UDF_BCL_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == UDFSqlContext.AddOneStatic(UDFSqlContext.CustomerOrderCountWithClientStatic(Math.Abs(c.Id)))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_BCL_Client_UDF_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == Math.Abs(UDFSqlContext.AddOneStatic(UDFSqlContext.CustomerOrderCountWithClientStatic(c.Id)))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_BCL_UDF_Client_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 1 == Math.Abs(UDFSqlContext.CustomerOrderCountWithClientStatic(UDFSqlContext.AddOneStatic(c.Id)))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_UDF_BCL_Client_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 1 == UDFSqlContext.CustomerOrderCountWithClientStatic(Math.Abs(UDFSqlContext.AddOneStatic(c.Id)))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_UDF_Client_BCL_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 1 == UDFSqlContext.CustomerOrderCountWithClientStatic(UDFSqlContext.AddOneStatic(Math.Abs(c.Id)))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_BCL_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == UDFSqlContext.AddOneStatic(Math.Abs(c.Id))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_UDF_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == UDFSqlContext.AddOneStatic(UDFSqlContext.CustomerOrderCountWithClientStatic(c.Id))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_BCL_Client_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == Math.Abs(UDFSqlContext.AddOneStatic(c.Id))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

#if !Test20
        [Fact]
        public void Scalar_Nested_Function_BCL_UDF_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == Math.Abs(UDFSqlContext.CustomerOrderCountStatic(c.Id))
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE 3 = ABS([dbo].[CustomerOrderCount]([c].[Id]))");
            }
        }
#endif

        [Fact]
        public void Scalar_Nested_Function_UDF_Client_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == UDFSqlContext.CustomerOrderCountWithClientStatic(UDFSqlContext.AddOneStatic(c.Id))
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

#if !Test20
        [Fact]
        public void Scalar_Nested_Function_UDF_BCL_Static()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == UDFSqlContext.CustomerOrderCountStatic(Math.Abs(c.Id))
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE 3 = [dbo].[CustomerOrderCount](ABS([c].[Id]))");
            }
        }
#endif

        [Fact]
        public void Nullable_navigation_property_access_preserves_schema_for_sql_function()
        {
            using (var context = CreateContext())
            {
                var result = context.Orders
                    .OrderBy(o => o.Id)
                    .Select(o => UDFSqlContext.IdentityString(o.Customer.FirstName))
                    .FirstOrDefault();

                Assert.Equal("Customer", result);
                AssertSql(
                    @"SELECT TOP(1) [dbo].[IdentityString]([o.Customer].[FirstName])
FROM [Orders] AS [o]
LEFT JOIN [Customers] AS [o.Customer] ON [o].[CustomerId] = [o.Customer].[Id]
ORDER BY [o].[Id]");
            }
        }

        #endregion

        #region Instance

#if !Test20
        [Fact]
        public void Scalar_Function_Non_Static()
        {
            using (var context = CreateContext())
            {
                var custName = (from c in context.Customers
                                where c.Id == 1
                                select new
                                {
                                    Id = context.StarValueInstance(4, c.Id),
                                    LastName = context.DollarValueInstance(2, c.LastName)
                                }).Single();

                Assert.Equal(custName.LastName, "$$One");

                AssertSql(
                    @"SELECT TOP(2) [dbo].[StarValue](4, [c].[Id]) AS [Id], [dbo].[DollarValue](2, [c].[LastName]) AS [LastName]
FROM [Customers] AS [c]
WHERE [c].[Id] = 1");
            }
        }
#endif

        [Fact]
        private void Scalar_Function_Extension_Method_Instance()
        {
            using (var context = CreateContext())
            {
                var len = context.Customers.Count(c => context.IsDateInstance(c.FirstName) == false);

                Assert.Equal(4, len);

                AssertSql(
                    @"SELECT COUNT(*)
FROM [Customers] AS [c]
WHERE CASE
    WHEN IsDate([c].[FirstName]) = 1
    THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT)
END = 0");
            }
        }

        [Fact]
        private void Scalar_Function_With_Translator_Translates_Instance()
        {
            using (var context = CreateContext())
            {
                var customerId = 3;

                var len = context.Customers.Where(c => c.Id == customerId)
                    .Select(c => context.MyCustomLengthInstance(c.LastName)).Single();

                Assert.Equal(5, len);

                AssertSql(
                    @"@__customerId_0='3'

SELECT TOP(2) len([c].[LastName])
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_0");
            }
        }

#if !Test20
        [Fact]
        public void Scalar_Function_ClientEval_Method_As_Translateable_Method_Parameter_Instance()
        {
            using (var context = CreateContext())
            {
                Assert.Throws<NotImplementedException>(
                    () => (from c in context.Customers
                           where c.Id == 1
                           select new
                           {
                               c.FirstName,
                               OrderCount = context.CustomerOrderCountInstance(context.AddFiveInstance(c.Id - 5))
                           }).Single());
            }
        }

        [Fact]
        public void Scalar_Function_Constant_Parameter_Instance()
        {
            using (var context = CreateContext())
            {
                var customerId = 1;

                var custs = context.Customers.Select(c => context.CustomerOrderCountInstance(customerId)).ToList();

                Assert.Equal(4, custs.Count);

                AssertSql(
                    @"@__customerId_1='1'

SELECT [dbo].[CustomerOrderCount](@__customerId_1)
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Correlated_Instance()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            where c.Id == 1
                            select new
                            {
                                c.LastName,
                                OrderCount = context.CustomerOrderCountInstance(c.Id)
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal(3, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount]([c].[Id]) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 1");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Not_Correlated_Instance()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            where c.Id == 1
                            select new
                            {
                                c.LastName,
                                OrderCount = context.CustomerOrderCountInstance(1)
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal(3, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](1) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 1");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Parameter_Instance()
        {
            using (var context = CreateContext())
            {
                var customerId = 1;

                var cust = (from c in context.Customers
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = context.CustomerOrderCountInstance(customerId)
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal(3, cust.OrderCount);

                AssertSql(
                    @"@__customerId_2='1'
@__customerId_0='1'

SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](@__customerId_2) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_0");
            }
        }

        [Fact]
        public void Scalar_Function_Anonymous_Type_Select_Nested_Instance()
        {
            using (var context = CreateContext())
            {
                var customerId = 3;
                var starCount = 3;

                var cust = (from c in context.Customers
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = context.StarValueInstance(starCount, context.CustomerOrderCountInstance(customerId))
                            }).Single();

                Assert.Equal("Three", cust.LastName);
                Assert.Equal("***1", cust.OrderCount);

                AssertSql(
                    @"@__starCount_2='3'
@__customerId_4='3'
@__customerId_0='3'

SELECT TOP(2) [c].[LastName], [dbo].[StarValue](@__starCount_2, [dbo].[CustomerOrderCount](@__customerId_4)) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_0");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Correlated_Instance()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            where context.IsTopCustomerInstance(c.Id)
                            select c.Id.ToString().ToLower()).ToList();

                Assert.Equal(1, cust.Count);

                AssertSql(
                    @"SELECT LOWER(CONVERT(VARCHAR(11), [c].[Id]))
FROM [Customers] AS [c]
WHERE [dbo].[IsTopCustomer]([c].[Id]) = 1");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Not_Correlated_Instance()
        {
            using (var context = CreateContext())
            {
                var startDate = new DateTime(2000, 4, 1);

                var custId = (from c in context.Customers
                              where context.GetCustomerWithMostOrdersAfterDateInstance(startDate) == c.Id
                              select c.Id).SingleOrDefault();

                Assert.Equal(custId, 2);

                AssertSql(
                    @"@__startDate_1='2000-04-01T00:00:00'

SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE [dbo].[GetCustomerWithMostOrdersAfterDate](@__startDate_1) = [c].[Id]");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Parameter_Instance()
        {
            using (var context = CreateContext())
            {
                var period = UDFSqlContext.ReportingPeriod.Winter;

                var custId = (from c in context.Customers
                              where c.Id == context.GetCustomerWithMostOrdersAfterDateInstance(
                                        context.GetReportingPeriodStartDateInstance(period))
                              select c.Id).SingleOrDefault();

                Assert.Equal(custId, 1);

                AssertSql(
                    @"@__period_2='0'

SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE [c].[Id] = [dbo].[GetCustomerWithMostOrdersAfterDate]([dbo].[GetReportingPeriodStartDate](@__period_2))");
            }
        }

        [Fact]
        public void Scalar_Function_Where_Nested_Instance()
        {
            using (var context = CreateContext())
            {
                var custId = (from c in context.Customers
                              where c.Id == context.GetCustomerWithMostOrdersAfterDateInstance(
                                        context.GetReportingPeriodStartDateInstance(
                                            UDFSqlContext.ReportingPeriod.Winter))
                              select c.Id).SingleOrDefault();

                Assert.Equal(custId, 1);

                AssertSql(
                    @"SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE [c].[Id] = [dbo].[GetCustomerWithMostOrdersAfterDate]([dbo].[GetReportingPeriodStartDate](0))");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Correlated_Instance()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            let orderCount = context.CustomerOrderCountInstance(c.Id)
                            where c.Id == 2
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("Two", cust.LastName);
                Assert.Equal(2, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount]([c].[Id]) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 2");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Not_Correlated_Instance()
        {
            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            let orderCount = context.CustomerOrderCountInstance(2)
                            where c.Id == 2
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("Two", cust.LastName);
                Assert.Equal(2, cust.OrderCount);

                AssertSql(
                    @"SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](2) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = 2");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Not_Parameter_Instance()
        {
            var customerId = 2;

            using (var context = CreateContext())
            {
                var cust = (from c in context.Customers
                            let orderCount = context.CustomerOrderCountInstance(customerId)
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("Two", cust.LastName);
                Assert.Equal(2, cust.OrderCount);

                AssertSql(
                    @"@__8__locals1_customerId_1='2'
@__8__locals1_customerId_2='2'

SELECT TOP(2) [c].[LastName], [dbo].[CustomerOrderCount](@__8__locals1_customerId_1) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__8__locals1_customerId_2");
            }
        }

        [Fact]
        public void Scalar_Function_Let_Nested_Instance()
        {
            using (var context = CreateContext())
            {
                var customerId = 1;
                var starCount = 3;

                var cust = (from c in context.Customers
                            let orderCount = context.StarValueInstance(starCount, context.CustomerOrderCountInstance(customerId))
                            where c.Id == customerId
                            select new
                            {
                                c.LastName,
                                OrderCount = orderCount
                            }).Single();

                Assert.Equal("One", cust.LastName);
                Assert.Equal("***3", cust.OrderCount);

                AssertSql(
                    @"@__starCount_1='3'
@__customerId_3='1'
@__customerId_4='1'

SELECT TOP(2) [c].[LastName], [dbo].[StarValue](@__starCount_1, [dbo].[CustomerOrderCount](@__customerId_3)) AS [OrderCount]
FROM [Customers] AS [c]
WHERE [c].[Id] = @__customerId_4");
            }
        }
#endif

        [Fact]
        public void Scalar_Nested_Function_Unwind_Client_Eval_Where_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == context.AddOneInstance(c.Id)
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Unwind_Client_Eval_OrderBy_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               orderby context.AddOneInstance(c.Id)
                               select c.Id).ToList();

                Assert.Equal(4, results.Count);
                Assert.True(results.SequenceEqual(Enumerable.Range(1, 4)));

                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Unwind_Client_Eval_Select_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               orderby c.Id
                               select context.AddOneInstance(c.Id)).ToList();

                Assert.Equal(4, results.Count);
                Assert.True(results.SequenceEqual(Enumerable.Range(2, 4)));

                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]
ORDER BY [c].[Id]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_BCL_UDF_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == context.AddOneInstance(Math.Abs(context.CustomerOrderCountWithClientInstance(c.Id)))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_UDF_BCL_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == context.AddOneInstance(context.CustomerOrderCountWithClientInstance(Math.Abs(c.Id)))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_BCL_Client_UDF_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == Math.Abs(context.AddOneInstance(context.CustomerOrderCountWithClientInstance(c.Id)))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_BCL_UDF_Client_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 1 == Math.Abs(context.CustomerOrderCountWithClientInstance(context.AddOneInstance(c.Id)))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_UDF_BCL_Client_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 1 == context.CustomerOrderCountWithClientInstance(Math.Abs(context.AddOneInstance(c.Id)))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_UDF_Client_BCL_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 1 == context.CustomerOrderCountWithClientInstance(context.AddOneInstance(Math.Abs(c.Id)))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_BCL_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == context.AddOneInstance(Math.Abs(c.Id))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_Client_UDF_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == context.AddOneInstance(context.CustomerOrderCountWithClientInstance(c.Id))
                               select c.Id).Single();

                Assert.Equal(3, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_BCL_Client_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == Math.Abs(context.AddOneInstance(c.Id))
                               select c.Id).Single();

                Assert.Equal(2, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

#if !Test20
        [Fact]
        public void Scalar_Nested_Function_BCL_UDF_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == Math.Abs(context.CustomerOrderCountInstance(c.Id))
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE 3 = ABS([dbo].[CustomerOrderCount]([c].[Id]))");
            }
        }


        [Fact]
        public void Scalar_Nested_Function_UDF_Client_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 2 == context.CustomerOrderCountWithClientInstance(context.AddOneInstance(c.Id))
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT [c].[Id]
FROM [Customers] AS [c]");
            }
        }

        [Fact]
        public void Scalar_Nested_Function_UDF_BCL_Instance()
        {
            using (var context = CreateContext())
            {
                var results = (from c in context.Customers
                               where 3 == context.CustomerOrderCountInstance(Math.Abs(c.Id))
                               select c.Id).Single();

                Assert.Equal(1, results);
                AssertSql(
                    @"SELECT TOP(2) [c].[Id]
FROM [Customers] AS [c]
WHERE 3 = [dbo].[CustomerOrderCount](ABS([c].[Id]))");
            }
        }
#endif

        #endregion

        #region BootStrap

        [Fact]
        public void BootstrapScalarNoParams()
        {
            using (var context = CreateContext())
            {
                var schame = context.SCHEMA_NAME();

                Assert.Equal("dbo", schame);

                AssertSql(
                    @"SELECT SCHEMA_NAME()");
            }
        }

        [Fact]
        public void BootstrapScalarParams()
        {
            using (var context = CreateContext())
            {
                var value = context.AddValues(1, 2);

                Assert.Equal(3, value);

                AssertSql(@"@__a_0='1'
@__b_1='2'

SELECT [dbo].[AddValues](@__a_0, @__b_1)");
            }
        }

        [Fact]
        public void BootstrapScalarFuncParams()
        {
            using (var context = CreateContext())
            {
                //    var a1 = context.AddValues(1, 2);
                //     var a = context.AddValues(() => 1, 2);
                //var x = 5;
                //var value = context.AddValues(() => context.AddValues(x, 2), 2);

                var value = context.AddValues(() => context.AddValues(1, 2), 2);

                Assert.Equal(5, value);

                AssertSql(@"@__b_1='2'

SELECT [dbo].[AddValues]([dbo].[AddValues](1, 2), @__b_1)");
            }
        }

        [Fact]
        public void BootstrapScalarFuncParamsWithVariable()
        {
            using (var context = CreateContext())
            {
                var x = 5;
                var value = context.AddValues(() => context.AddValues(x, 2), 2);

                Assert.Equal(9, value);

                AssertSql(@"@__x_1='5'
@__b_2='2'

SELECT [dbo].[AddValues]([dbo].[AddValues](@__x_1, 2), @__b_2)");
            }
        }

        [Fact]
        public void BootstrapScalarFuncParamsConstant()
        {
            using (var context = CreateContext())
            {
                var value = context.AddValues(() => 1, 2);

                Assert.Equal(3, value);

                AssertSql(@"@__b_0='2'

SELECT [dbo].[AddValues](1, @__b_0)");
            }
        }
        #endregion

        #endregion

        #region Table Valued Tests

        [Fact]
        public void TV_Function_Stand_Alone()
        {
            using (var context = CreateContext())
            {
                var products = (from t in context.GetTopTwoSellingProducts()
                                orderby t.ProductId
                                select t).ToList();

                Assert.Equal(2, products.Count);
                Assert.Equal(1, products[0].ProductId);
                Assert.Equal(27, products[0].AmountSold);
                Assert.Equal(2, products[1].ProductId);
                Assert.Equal(50, products[1].AmountSold);

                AssertSql(@"SELECT [t].[AmountSold], [t].[ProductId]
FROM [dbo].[GetTopTwoSellingProducts]() AS [t]
ORDER BY [t].[ProductId]");
            }
        }

        [Fact]
        public void TV_Function_Stand_Alone_Parameter()
        {
            using (var context = CreateContext())
            {
                var orders = (from c in context.GetCustomerOrderCountByYear(1)
                              orderby c.Count descending
                              select c).ToList();

                Assert.Equal(2, orders.Count);
                Assert.Equal(2, orders[0].Count);
                Assert.Equal(2000, orders[0].Year);
                Assert.Equal(1, orders[1].Count);
                Assert.Equal(2001, orders[1].Year);

                AssertSql(@"@__customerId_0='1'

SELECT [c].[Count], [c].[CustomerId], [c].[Year]
FROM [dbo].[GetCustomerOrderCountByYear](@__customerId_0) AS [c]
ORDER BY [c].[Count] DESC");
            }
        }

        [Fact]
        public void TV_Function_Stand_Alone_Nested()
        {
            using (var context = CreateContext())
            {
                var orders = (from r in context.GetCustomerOrderCountByYear(() => context.AddValues(-2, 3))
                              orderby r.Count descending
                              select r).ToList();

                Assert.Equal(2, orders.Count);
                Assert.Equal(2, orders[0].Count);
                Assert.Equal(2000, orders[0].Year);
                Assert.Equal(1, orders[1].Count);
                Assert.Equal(2001, orders[1].Year);

                AssertSql(@"SELECT [r].[Count], [r].[CustomerId], [r].[Year]
FROM [dbo].[GetCustomerOrderCountByYear]([dbo].[AddValues](-2, 3)) AS [r]
ORDER BY [r].[Count] DESC");
            }
        }

        [Fact]
        public void TV_Function_CrossApply_Correlated_Select_Anonymous()
        {
            using (var context = CreateContext())
            {
                var orders = (from c in context.Customers
                              from r in context.GetCustomerOrderCountByYear(c.Id)
                              orderby c.Id, r.Year
                              select new
                              {
                                  c.Id,
                                  c.LastName,
                                  r.Year,
                                  r.Count
                              }).ToList();

                Assert.Equal(4, orders.Count);
                Assert.Equal(2, orders[0].Count);
                Assert.Equal(1, orders[1].Count);
                Assert.Equal(2, orders[2].Count);
                Assert.Equal(1, orders[3].Count);
                Assert.Equal(2000, orders[0].Year);
                Assert.Equal(2001, orders[1].Year);
                Assert.Equal(2000, orders[2].Year);
                Assert.Equal(2001, orders[3].Year);
                Assert.Equal(1, orders[0].Id);
                Assert.Equal(1, orders[1].Id);
                Assert.Equal(2, orders[2].Id);
                Assert.Equal(3, orders[3].Id);

                AssertSql(@"SELECT [c].[Id], [c].[LastName], [r].[Year], [r].[Count]
FROM [Customers] AS [c]
CROSS APPLY [dbo].[GetCustomerOrderCountByYear]([c].[Id]) AS [r]
ORDER BY [c].[Id], [r].[Year]");
            }
        }

        [Fact]
        public void Table_Function_CrossApply_Correlated_Select_Result()
        {
            using (var context = CreateContext())
            {
                var orders = (from c in context.Customers
                              from r in context.GetCustomerOrderCountByYear(c.Id)
                              orderby r.Count descending, r.Year descending
                              select r).ToList();

                Assert.Equal(4, orders.Count);

                Assert.Equal(4, orders.Count);
                Assert.Equal(2, orders[0].Count);
                Assert.Equal(2, orders[1].Count);
                Assert.Equal(1, orders[2].Count);
                Assert.Equal(1, orders[3].Count);
                Assert.Equal(2000, orders[0].Year);
                Assert.Equal(2000, orders[1].Year);
                Assert.Equal(2001, orders[2].Year);
                Assert.Equal(2001, orders[3].Year);

                AssertSql(@"SELECT [r].[Count], [r].[CustomerId], [r].[Year]
FROM [Customers] AS [c]
CROSS APPLY [dbo].[GetCustomerOrderCountByYear]([c].[Id]) AS [r]
ORDER BY [r].[Count] DESC, [r].[Year] DESC");
            }
        }

        [Fact]
        public void TV_Function_CrossJoin_Not_Correlated()
        {
            using (var context = CreateContext())
            {
                var orders = (from c in context.Customers
                              from r in context.GetCustomerOrderCountByYear(2)
                              where c.Id == 2
                              orderby r.Count
                              select new
                              {
                                  c.Id,
                                  c.LastName,
                                  r.Year,
                                  r.Count
                              }).ToList();

                Assert.Equal(1, orders.Count);

                Assert.Equal(2, orders[0].Count);
                Assert.Equal(2000, orders[0].Year);

                AssertSql(@"SELECT [c].[Id], [c].[LastName], [r].[Year], [r].[Count]
FROM [Customers] AS [c]
CROSS JOIN [dbo].[GetCustomerOrderCountByYear](2) AS [r]
WHERE [c].[Id] = 2
ORDER BY [r].[Count]");
            }
        }

        [Fact]
        public void TV_Function_CrossJoin_Parameter()
        {
            using (var context = CreateContext())
            {
                var custId = 2;

                var orders = (from c in context.Customers
                              from r in context.GetCustomerOrderCountByYear(custId)
                              where c.Id == custId
                              orderby r.Count
                              select new
                              {
                                  c.Id,
                                  c.LastName,
                                  r.Year,
                                  r.Count
                              }).ToList();

                Assert.Equal(1, orders.Count);

                Assert.Equal(2, orders[0].Count);
                Assert.Equal(2000, orders[0].Year);

                AssertSql(@"@__custId_1='2'
@__custId_2='2'

SELECT [c].[Id], [c].[LastName], [r].[Year], [r].[Count]
FROM [Customers] AS [c]
CROSS JOIN [dbo].[GetCustomerOrderCountByYear](@__custId_1) AS [r]
WHERE [c].[Id] = @__custId_2
ORDER BY [r].[Count]");
            }
        }

        [Fact]
        public void TV_Function_Join()
        {
            //performing a join requires the method to have a body which calls ExecuteTableValuedFunction
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopTwoSellingProducts() on p.Id equals r.ProductId
                                select new
                                {
                                    p.Id,
                                    p.Name,
                                    r.AmountSold
                                }).OrderBy(p => p.Id).ToList();

                Assert.Equal(2, products.Count);
                Assert.Equal(1, products[0].Id);
                Assert.Equal("Product1", products[0].Name);
                Assert.Equal(27, products[0].AmountSold);
                Assert.Equal(2, products[1].Id);
                Assert.Equal("Product2", products[1].Name);
                Assert.Equal(50, products[1].AmountSold);

                AssertSql(@"SELECT [p].[Id], [p].[Name], [r].[AmountSold]
FROM [Products] AS [p]
INNER JOIN [dbo].[GetTopTwoSellingProducts]() AS [r] ON [p].[Id] = [r].[ProductId]
ORDER BY [p].[Id]");

            }
        }

        [Fact]
        public void TV_Function_LeftJoin_Select_Anonymous()
        {
            //performing a join requires the method to have a body which calls ExecuteTableValuedFunction
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopTwoSellingProducts() on p.Id equals r.ProductId into joinTable
                                from j in joinTable.DefaultIfEmpty()
                                orderby p.Id descending
                                select new
                                {
                                    p.Id,
                                    p.Name,
                                    j.AmountSold
                                }).ToList();

                Assert.Equal(5, products.Count);
                Assert.Equal(5, products[0].Id);
                Assert.Equal(null, products[0].AmountSold);
                Assert.Equal("Product5", products[0].Name);
                Assert.Equal(4, products[1].Id);
                Assert.Equal(null, products[1].AmountSold);
                Assert.Equal("Product4", products[1].Name);
                Assert.Equal(3, products[2].Id);
                Assert.Equal(null, products[2].AmountSold);
                Assert.Equal("Product3", products[2].Name);
                Assert.Equal(2, products[3].Id);
                Assert.Equal(50, products[3].AmountSold);
                Assert.Equal("Product2", products[3].Name);
                Assert.Equal(1, products[4].Id);
                Assert.Equal(27, products[4].AmountSold);
                Assert.Equal("Product1", products[4].Name);

                AssertSql(@"SELECT [p].[Id], [p].[Name], [r].[AmountSold]
FROM [Products] AS [p]
LEFT JOIN [dbo].[GetTopTwoSellingProducts]() AS [r] ON [p].[Id] = [r].[ProductId]
ORDER BY [p].[Id] DESC");
            }
        }

        [Fact]
        public void TV_Function_LeftJoin_Select_Result()
        {
            //performing a join requires the method to have a body which calls ExecuteTableValuedFunction
            using (var context = CreateContext())
            {
                var products = (from p in context.Products
                                join r in context.GetTopTwoSellingProducts() on p.Id equals r.ProductId into joinTable
                                from j in joinTable.DefaultIfEmpty()
                                orderby p.Id descending
                                select j).ToList();

                Assert.Equal(5, products.Count);
                Assert.Equal(null, products[0].ProductId);
                Assert.Equal(null, products[0].AmountSold);
                Assert.Equal(null, products[1].ProductId);
                Assert.Equal(null, products[1].AmountSold);
                Assert.Equal(null, products[2].ProductId);
                Assert.Equal(null, products[2].AmountSold);
                Assert.Equal(2, products[3].ProductId);
                Assert.Equal(50, products[3].AmountSold);
                Assert.Equal(1, products[4].ProductId);
                Assert.Equal(27, products[4].AmountSold);

                AssertSql(@"SELECT [r].[AmountSold], [r].[ProductId]
FROM [Products] AS [p]
LEFT JOIN [dbo].[GetTopTwoSellingProducts]() AS [r] ON [p].[Id] = [r].[ProductId]
ORDER BY [p].[Id] DESC");

            }
        }

        [Fact]
        public void TV_Function_OuterApply_Correlated_Select_TVF()
        {
            using (var context = CreateContext())
            {
                var orders = (from c in context.Customers
                              from r in context.GetCustomerOrderCountByYear(c.Id).DefaultIfEmpty()
                              orderby c.Id, r.Year
                              select r).ToList();

                /* 
                 select new
                 {
                     c.Id,
                     c.LastName,
                     r.Year,
                     r.Count
                 }).ToList();*/

                Assert.Equal(5, orders.Count);

                Assert.Equal(2, orders[0].Count);
                Assert.Equal(1, orders[1].Count);
                Assert.Equal(2, orders[2].Count);
                Assert.Equal(1, orders[3].Count);
                Assert.Null(orders[4].Count);
                Assert.Equal(2000, orders[0].Year);
                Assert.Equal(2001, orders[1].Year);
                Assert.Equal(2000, orders[2].Year);
                Assert.Equal(2001, orders[3].Year);
                Assert.Null(orders[4].Year);
                Assert.Equal(1, orders[0].CustomerId);
                Assert.Equal(1, orders[1].CustomerId);
                Assert.Equal(2, orders[2].CustomerId);
                Assert.Equal(3, orders[3].CustomerId);
                Assert.Null(orders[4].CustomerId);

                AssertSql(@"SELECT [g].[Count], [g].[CustomerId], [g].[Year]
FROM [Customers] AS [c]
OUTER APPLY [dbo].[GetCustomerOrderCountByYear]([c].[Id]) AS [g]
ORDER BY [c].[Id], [g].[Year]");
            }
        }

        [Fact]
        public void TV_Function_Nested()
        {
            //TODO - this is selecting too many employee columns in the query
            using (var context = CreateContext())
            {
                var custId = 2;

                var orders = (from c in context.Customers
                              from r in context.GetCustomerOrderCountByYear(context.AddValues(1, 1))
                              where c.Id == custId
                              orderby r.Year
                              select new
                              {
                                  c.Id,
                                  c.LastName,
                                  r.Year,
                                  r.Count
                              }).ToList();

                Assert.Equal(1, orders.Count);

                Assert.Equal(2, orders[0].Count);
                Assert.Equal(2000, orders[0].Year);

                AssertSql(@"@__custId_2='2'

SELECT [c].[Id], [c].[LastName], [r].[Year], [r].[Count]
FROM [Customers] AS [c]
CROSS JOIN [dbo].[GetCustomerOrderCountByYear]([dbo].[AddValues](1, 1)) AS [r]
WHERE [c].[Id] = @__custId_2
ORDER BY [r].[Year]");
            }
        }

        /*
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

        #endregion

        public class SqlServerUDFFixture : SharedStoreFixtureBase<DbContext>
        {
            protected override string StoreName { get; } = "UDFDbFunctionSqlServerTests";
            protected override Type ContextType { get; } = typeof(UDFSqlContext);
            protected override ITestStoreFactory TestStoreFactory => SqlServerTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                base.AddOptions(builder);
                return builder.ConfigureWarnings(w => w.Ignore(RelationalEventId.QueryClientEvaluationWarning));
            }

            protected override void Seed(DbContext context)
            {
                context.Database.EnsureCreated();

                context.Database.ExecuteSqlCommand(@"create function [dbo].[CustomerOrderCount] (@customerId int)
                                                    returns int
                                                    as
                                                    begin
                                                        return (select count(id) from orders where customerId = @customerId);
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function[dbo].[StarValue] (@starCount int, @value nvarchar(max))
                                                    returns nvarchar(max)
                                                        as
                                                        begin
                                                    return replicate('*', @starCount) + @value
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function[dbo].[DollarValue] (@starCount int, @value nvarchar(max))
                                                    returns nvarchar(max)
                                                        as
                                                        begin
                                                    return replicate('$', @starCount) + @value
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].[GetReportingPeriodStartDate] (@period int)
                                                    returns DateTime
                                                    as
                                                    begin
                                                        return '1998-01-01'
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].[GetCustomerWithMostOrdersAfterDate] (@searchDate Date)
                                                    returns int
                                                    as
                                                    begin
                                                        return (select top 1 customerId
                                                                from orders
                                                                where orderDate > @searchDate
                                                                group by CustomerId
                                                                order by count(id) desc)
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].[IsTopCustomer] (@customerId int)
                                                    returns bit
                                                    as
                                                    begin
                                                        if(@customerId = 1)
                                                            return 1

                                                        return 0
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].[IdentityString] (@customerName nvarchar(max))
                                                    returns nvarchar(max)
                                                    as
                                                    begin
                                                        return @customerName;
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].GetCustomerOrderCountByYear(@customerId int)
                                                    returns @reports table
                                                    (
                                                        CustomerId int not null,
	                                                    Count int not null,
	                                                    Year int not null
                                                    )
                                                    as
                                                    begin
	
	                                                    insert into @reports
	                                                    select @customerId, count(id), year(orderDate)
	                                                    from orders
	                                                    where customerId = @customerId
	                                                    group by customerId, year(orderDate)
	                                                    order by year(orderDate)
	
	                                                    return 
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].GetTopTwoSellingProducts()
                                                    returns @products table
                                                    (
	                                                    ProductId int not null,
	                                                    AmountSold int
                                                    )
                                                    as
                                                    begin
	
	                                                    insert into @products
	                                                    select top 2 ProductID, sum(quantitySold) as totalSold
	                                                    from orders
	                                                    group by ProductID
	                                                    order by totalSold desc

	                                                    return 
                                                    end");

                context.Database.ExecuteSqlCommand(@"create function [dbo].[AddValues] (@a int, @b int)
                                                    returns int
                                                    as
                                                    begin
                                                        return @a + @b;
                                                    end");

                var product1 = new Product { Name = "Product1" };
                var product2 = new Product { Name = "Product2" };
                var product3 = new Product { Name = "Product3" };
                var product4 = new Product { Name = "Product4" };
                var product5 = new Product { Name = "Product5" };

                var order11 = new Order { Name = "Order11", QuantitySold = 4, OrderDate = new DateTime(2000, 1, 20), Product = product1 };
                var order12 = new Order { Name = "Order12", QuantitySold = 8, OrderDate = new DateTime(2000, 2, 21), Product = product2 };
                var order13 = new Order { Name = "Order13", QuantitySold = 15, OrderDate = new DateTime(2001, 3, 20), Product = product3 };
                var order21 = new Order { Name = "Order21", QuantitySold = 16, OrderDate = new DateTime(2000, 4, 21), Product = product4 };
                var order22 = new Order { Name = "Order22", QuantitySold = 23, OrderDate = new DateTime(2000, 5, 20), Product = product1 };
                var order31 = new Order { Name = "Order31", QuantitySold = 42, OrderDate = new DateTime(2001, 6, 21), Product = product2 };

                var customer1 = new Customer { FirstName = "Customer", LastName = "One", Orders = new List<Order> { order11, order12, order13 } };
                var customer2 = new Customer { FirstName = "Customer", LastName = "Two", Orders = new List<Order> { order21, order22 } };
                var customer3 = new Customer { FirstName = "Customer", LastName = "Three", Orders = new List<Order> { order31 } };
                var customer4 = new Customer { FirstName = "Customer", LastName = "Four" };

                ((UDFSqlContext)context).Customers.AddRange(customer1, customer2, customer3, customer4);
                ((UDFSqlContext)context).Orders.AddRange(order11, order12, order13, order21, order22, order31);
                ((UDFSqlContext)context).Products.AddRange(product1, product2, product3, product4, product5);
                context.SaveChanges();
            }
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
