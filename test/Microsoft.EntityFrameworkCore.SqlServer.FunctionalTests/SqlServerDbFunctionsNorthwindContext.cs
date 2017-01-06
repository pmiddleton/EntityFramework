// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Specification.Tests.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.SqlServer.FunctionalTests.TestModels
{
    public class SqlServerDbFunctionsNorthwindContext : SqlServerNorthwindContext
    {
        public enum ReportingPeriod
        {
            Winter = 0,
            Spring,
            Summer,
            Fall
        }

        public SqlServerDbFunctionsNorthwindContext(DbContextOptions options,
            QueryTrackingBehavior queryTrackingBehavior = QueryTrackingBehavior.TrackAll)
            : base(options, queryTrackingBehavior)
        {
        }

        [DbFunction("dbo", "EmployeeOrderCount")]
        public int EmployeeOrderCount(int employeeId)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Schema = "dbo")]
        public bool IsTopEmployee(int employeeId)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Schema = "dbo")]
        public int GetEmployeeWithMostOrdersAfterDate(DateTime? startDate)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Schema = "dbo")]
        public DateTime? GetReportingPeriodStartDate(ReportingPeriod periodId)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Schema = "dbo")]
        public string StarValue(int starCount, int value)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Schema = "dbo")]
        public int AddValues(int a, int b)
        {
            throw new NotImplementedException();
        }

        [DbFunction(Schema = "dbo")]
        public DateTime GetBestYearEver()
        {
            throw new NotImplementedException();
        }

        public class ManagersEmployee
        {
            [Key]
            public int EmployeeId { get; set; }
            public int? ManagerId { get; set; }
        }

        [DbFunction(Schema = "dbo")]
        public IQueryable<ManagersEmployee> FindReportsForManager()
        {
            return ExecuteTableValuedFunction<ManagersEmployee>(typeof(SqlServerDbFunctionsNorthwindContext).GetTypeInfo().GetDeclaredMethod(nameof(FindReportsForManager)));
        }


        public class OrderByYear
        {
            [Key]
            public int OrderCount { get; set; }
            public int Year { get; set; }
        }

        [DbFunction(Schema = "dbo")]
        public IQueryable<OrderByYear> GetEmployeeOrderCountByYear(int employeeId)
        {
            return ExecuteTableValuedFunction<OrderByYear>(new object[] { employeeId });
            //return ExecuteTableValuedFunction<OrderByYear>(typeof(SqlServerDbFunctionsNorthwindContext).GetMethod(nameof(GetEmployeeOrderCountByYear)), employeeId);
        }

        //[DbFunction(Schema = "dbo")]
        public IQueryable<OrderByYear> GetEmployeeOrderCountByYear(Expression<Func<int>> employeeId)
        {
            //var mi = typeof(SqlServerDbFunctionsNorthwindContext).GetMethod(nameof(GetEmployeeOrderCountByYear)) ;

            //return ExecuteTableValuedFunction<OrderByYear>(typeof(SqlServerDbFunctionsNorthwindContext).GetMethod(nameof(GetEmployeeOrderCountByYear)), employeeId);
            return ExecuteTableValuedFunction<OrderByYear>(new object[] { employeeId });
        }

        public class TopSellingProduct
        {
            [Key]
            public int? ProductId { get; set; }
            public int? AmountSold { get; set; }
        }

        [DbFunction(Schema = "dbo")]
        public IQueryable<TopSellingProduct> GetTopThreeSellingProducts()
        {
            return ExecuteTableValuedFunction<TopSellingProduct>(typeof(SqlServerDbFunctionsNorthwindContext).GetTypeInfo().GetDeclaredMethod(nameof(GetTopThreeSellingProducts)));
        }

        //[DbFunction(Schema = "dbo")]
        public IQueryable<TopSellingProduct> GetTopThreeSellingProductsForYear(Expression<Func<DateTime>> salesYear)
        {
            return ExecuteTableValuedFunction<TopSellingProduct>(new object[] { salesYear });
            //return ExecuteTableValuedFunction<TopSellingProduct>(typeof(SqlServerDbFunctionsNorthwindContext).GetMethod(nameof(GetTopThreeSellingProductsForYear)), salesYear);
            //return ExecuteTableValuedFunction<TopSellingProduct>(typeof(SqlServerDbFunctionsNorthwindContext).GetMethod(nameof(GetTopThreeSellingProducts)), salesYear);
        }

        [DbFunction(Schema = "dbo")]
        public IQueryable<TopSellingProduct> GetTopThreeSellingProductsForYear(DateTime salesYear)
        {
            return ExecuteTableValuedFunction<TopSellingProduct>(typeof(SqlServerDbFunctionsNorthwindContext).GetTypeInfo().GetDeclaredMethod(nameof(GetTopThreeSellingProducts)), salesYear);
        }

        public class LastOrderDetail
        {
            [Key]
            public int? OrderId { get; set; }
            public DateTime? OrderDate { get; set; }
        }

        [DbFunction(Schema = "dbo")]
        public IQueryable<LastOrderDetail> GetLatestNOrdersForCustomer(int lastNOrder, string customerId)
        {
            return ExecuteTableValuedFunction<LastOrderDetail>(typeof(SqlServerDbFunctionsNorthwindContext).GetTypeInfo().GetDeclaredMethod(nameof(GetLatestNOrdersForCustomer)), lastNOrder, customerId);
        }
    }
}
