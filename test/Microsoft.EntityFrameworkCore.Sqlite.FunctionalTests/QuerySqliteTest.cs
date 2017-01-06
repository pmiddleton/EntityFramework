// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Specification.Tests;
using Xunit;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore.Relational.Specification.Tests;

#if NETCOREAPP1_1
using System.Threading;
#endif
namespace Microsoft.EntityFrameworkCore.Sqlite.FunctionalTests
{
    public class QuerySqliteTest : QueryRelationalTestBase<NorthwindQuerySqliteFixture>
    {
        public QuerySqliteTest(NorthwindQuerySqliteFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //TestSqlLoggerFactory.CaptureOutput(testOutputHelper);
        }

        public override void Take_Skip()
        {
            base.Take_Skip();

            Assert.Contains(
                @"SELECT ""t"".*
FROM (
    SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
    FROM ""Customers"" AS ""c""
    ORDER BY ""c"".""ContactName""
    LIMIT @__p_0
) AS ""t""
ORDER BY ""t"".""ContactName""
LIMIT -1 OFFSET @__p_1",
                Sql);
        }

        public override void IsNullOrWhiteSpace_in_predicate()
        {
            base.IsNullOrWhiteSpace_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""Region"" IS NULL OR (trim(""c"".""Region"") = '')",
                Sql);
        }

        public override void TrimStart_in_predicate()
        {
            base.TrimStart_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ltrim(""c"".""ContactTitle"") = 'Owner'",
                Sql);
        }

        public override void TrimStart_with_arguments_in_predicate()
        {
            base.TrimStart_with_arguments_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ltrim(""c"".""ContactTitle"", 'Ow') = 'ner'",
                Sql);
        }

        public override void TrimEnd_in_predicate()
        {
            base.TrimEnd_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE rtrim(""c"".""ContactTitle"") = 'Owner'",
                Sql);
        }

        public override void TrimEnd_with_arguments_in_predicate()
        {
            base.TrimEnd_with_arguments_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE rtrim(""c"".""ContactTitle"", 'er') = 'Own'",
                Sql);
        }

        public override void Trim_in_predicate()
        {
            base.Trim_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE trim(""c"".""ContactTitle"") = 'Owner'",
                Sql);
        }

        public override void Trim_with_arguments_in_predicate()
        {
            base.Trim_with_arguments_in_predicate();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE trim(""c"".""ContactTitle"", 'Or') = 'wne'",
                Sql);
        }

        public override void Sum_with_coalesce()
        {
            base.Sum_with_coalesce();

            Assert.Contains(
                @"SELECT SUM(COALESCE(""p"".""UnitPrice"", 0.0))
FROM ""Products"" AS ""p""
WHERE ""p"".""ProductID"" < 40",
                Sql);
        }

        public override void String_Compare_nested()
        {
            base.String_Compare_nested();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""CustomerID"" = 'M' || ""c"".""CustomerID""", Sql);

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""CustomerID"" <> upper(""c"".""CustomerID"")", Sql);

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""CustomerID"" > REPLACE('ALFKI', upper('ALF'), ""c"".""CustomerID"")", Sql);

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""CustomerID"" <= 'M' || ""c"".""CustomerID""", Sql);

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""CustomerID"" > upper(""c"".""CustomerID"")", Sql);

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""CustomerID"" < REPLACE('ALFKI', upper('ALF'), ""c"".""CustomerID"")", Sql);
        }

        public override void String_StartsWith_Literal()
        {
            base.String_StartsWith_Literal();

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE ""c"".""ContactName"" LIKE 'M' || '%' AND (instr(""c"".""ContactName"", 'M') = 1)",
                Sql);
        }

        public override void String_StartsWith_Identity()
        {
            base.String_StartsWith_Identity();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (""c"".""ContactName"" LIKE ""c"".""ContactName"" || '%' AND (instr(""c"".""ContactName"", ""c"".""ContactName"") = 1)) OR (""c"".""ContactName"" = '')",
                Sql);
        }

        public override void String_StartsWith_Column()
        {
            base.String_StartsWith_Column();

            Assert.Contains(@"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (""c"".""ContactName"" LIKE ""c"".""ContactName"" || '%' AND (instr(""c"".""ContactName"", ""c"".""ContactName"") = 1)) OR (""c"".""ContactName"" = '')",
                Sql);
        }

        public override void String_StartsWith_MethodCall()
        {
            base.String_StartsWith_MethodCall();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (""c"".""ContactName"" LIKE @__LocalMethod1_0 || '%' AND (instr(""c"".""ContactName"", @__LocalMethod1_0) = 1)) OR (@__LocalMethod1_0 = '')",
                Sql);
        }

        public override void String_EndsWith_Literal()
        {
            base.String_EndsWith_Literal();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE substr(""c"".""ContactName"", -(length('b'))) = 'b'",
                Sql);
        }

        public override void String_EndsWith_Identity()
        {
            base.String_EndsWith_Identity();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (substr(""c"".""ContactName"", -(length(""c"".""ContactName""))) = ""c"".""ContactName"") OR (""c"".""ContactName"" = '')",
                Sql);
        }

        public override void String_EndsWith_Column()
        {
            base.String_EndsWith_Column();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (substr(""c"".""ContactName"", -(length(""c"".""ContactName""))) = ""c"".""ContactName"") OR (""c"".""ContactName"" = '')",
                Sql);
        }

        public override void String_EndsWith_MethodCall()
        {
            base.String_EndsWith_MethodCall();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (substr(""c"".""ContactName"", -(length(@__LocalMethod2_0))) = @__LocalMethod2_0) OR (@__LocalMethod2_0 = '')",
                Sql);
        }

        public override void String_Contains_Literal()
        {
            base.String_Contains_Literal();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE instr(""c"".""ContactName"", 'M') > 0",
                Sql);
        }

        public override void String_Contains_Identity()
        {
            base.String_Contains_Identity();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (instr(""c"".""ContactName"", ""c"".""ContactName"") > 0) OR (""c"".""ContactName"" = '')",
                Sql);
        }

        public override void String_Contains_Column()
        {
            base.String_Contains_Column();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (instr(""c"".""ContactName"", ""c"".""ContactName"") > 0) OR (""c"".""ContactName"" = '')",
                Sql);
        }

        public override void String_Contains_MethodCall()
        {
            base.String_Contains_MethodCall();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE (instr(""c"".""ContactName"", @__LocalMethod1_0) > 0) OR (@__LocalMethod1_0 = '')",
                Sql);
        }

        public override void Where_math_round()
        {
            base.Where_math_round();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE ROUND(""od"".""UnitPrice"", 0) > 10.0",
                Sql);
        }

        public override void Where_math_max()
        {
            base.Where_math_max();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE Max(""od"".""UnitPrice"", 5.0) > 10.0",
                Sql);
        }

        public override void Where_math_min()
        {
            base.Where_math_min();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE Min(""od"".""UnitPrice"", 50.0) > 10.0",
                Sql);
        }

        public override void Where_math_round_decimals()
        {
            base.Where_math_round_decimals();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE ROUND(""od"".""UnitPrice"", 1) > 15.0",
                Sql);
        }

        public override void Where_math_abs1()
        {
            base.Where_math_abs1();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE abs(""od"".""ProductID"") > 10",
                Sql);
        }

        public override void Where_math_abs2()
        {
            base.Where_math_abs2();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE abs(""od"".""Quantity"") > 10",
                Sql);
        }

        public override void Where_math_abs3()
        {
            base.Where_math_abs3();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE abs(""od"".""UnitPrice"") > 10.0",
                Sql);
        }

        public override void Where_math_abs_uncorrelated()
        {
            base.Where_math_abs_uncorrelated();

            Assert.Contains(
                @"SELECT ""od"".""OrderID"", ""od"".""ProductID"", ""od"".""Discount"", ""od"".""Quantity"", ""od"".""UnitPrice""
FROM ""Order Details"" AS ""od""
WHERE abs(-10) < ""od"".""ProductID""",
                Sql);
        }

        public override void Substring_with_constant()
        {
            base.Substring_with_constant();

            Assert.Contains(
                @"SELECT substr(""c"".""ContactName"", 2, 3)
FROM ""Customers"" AS ""c""
ORDER BY ""c"".""CustomerID""",
                Sql);
        }

        public override void Substring_with_closure()
        {
            base.Substring_with_closure();

            Assert.Contains(
                @"SELECT substr(""c"".""ContactName"", @__start_0 + 1, 3)
FROM ""Customers"" AS ""c""
ORDER BY ""c"".""CustomerID""",
                Sql);
        }

        public override void Substring_with_client_eval()
        {
            base.Substring_with_client_eval();

            Assert.Contains(
                @"SELECT ""c"".""ContactName""
FROM ""Customers"" AS ""c""
ORDER BY ""c"".""CustomerID""",
                Sql);
        }

        public override void Where_string_to_lower()
        {
            base.Where_string_to_lower();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE lower(""c"".""CustomerID"") = 'alfki'",
                Sql);
        }

        public override void Where_string_to_upper()
        {
            base.Where_string_to_upper();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID"", ""c"".""Address"", ""c"".""City"", ""c"".""CompanyName"", ""c"".""ContactName"", ""c"".""ContactTitle"", ""c"".""Country"", ""c"".""Fax"", ""c"".""Phone"", ""c"".""PostalCode"", ""c"".""Region""
FROM ""Customers"" AS ""c""
WHERE upper(""c"".""CustomerID"") = 'ALFKI'",
                Sql);
        }

        #region Date & Time Overloads

        public override void DateTime_AddYears()
        {
            base.DateTime_AddYears();

            Assert.Contains(
                @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%Y', datetime(""o"".""OrderDate"", 3 || ' year')) AS INTEGER) = 1999",
                Sql);
        }

        public override void DateTime_AddDays()
        {
            base.DateTime_AddDays();

            Assert.Contains(
              @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%d', datetime(""o"".""OrderDate"", 2E0 || ' day')) AS INTEGER) = 5",
              Sql);
        }

        public override void DateTime_AddHours()
        {
            base.DateTime_AddHours();

            Assert.Contains(
              @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%H', datetime(""o"".""OrderDate"", 11E0 || ' hour')) AS INTEGER) = 11",
              Sql);
        }

        public override void DateTime_AddMinutes()
        {
            base.DateTime_AddMinutes();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%M', datetime(""o"".""OrderDate"", 7E0 || ' minute')) AS INTEGER) = 7",
               Sql);
        }

        public override void DateTime_AddSeconds()
        {
            base.DateTime_AddSeconds();

            Assert.Contains(
              @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%S', datetime(""o"".""OrderDate"", 6E0 || ' second')) AS INTEGER) = 6",
              Sql);
        }

        public override void DateTime_Add_Chained()
        {
            base.DateTime_Add_Chained();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%S', datetime(datetime(datetime(datetime(datetime(""o"".""OrderDate"", 1 || ' year'), 1E0 || ' day'), 1E0 || ' hour'), 1E0 || ' minute'), 6E0 || ' second')) AS INTEGER) = 6",
               Sql);
        }


        #endregion

        #region DBFunctions

        public override void DbFunction_Left()
        {
            base.DbFunction_Left();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID""
FROM ""Customers"" AS ""c""
WHERE substr(""c"".""City"", 1, 3) = 'Sea'",
                Sql);
        }

        public override void DbFunction_Right()
        {
            base.DbFunction_Right();

            Assert.Contains(
                @"SELECT ""c"".""CustomerID""
FROM ""Customers"" AS ""c""
WHERE substr(""c"".""City"", -(2)) = 'le'",
                Sql);
        }

        public override void DbFunction_AddYears_Date()
        {
            base.DbFunction_AddYears_Date();

            Assert.Contains(
                @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%Y', datetime(""o"".""OrderDate"", 3 || ' year')) AS INTEGER) = 1999",
                Sql);
        }

        public override void DbFunction_AddMonths_Date()
        {
            base.DbFunction_AddMonths_Date();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%m', datetime(""o"".""OrderDate"", 2 || ' month')) AS INTEGER) = 5",
               Sql);
        }

        public override void DbFunction_AddDays_Date()
        {
            base.DbFunction_AddDays_Date();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%d', datetime(""o"".""OrderDate"", 2 || ' day')) AS INTEGER) = 5",
               Sql);
        }

        public override void DbFunction_AddHours_Date()
        {
            base.DbFunction_AddHours_Date();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%H', datetime(""o"".""OrderDate"", 11 || ' hour')) AS INTEGER) = 11",
               Sql);
        }

        public override void DbFunction_AddMinutes_Date()
        {
            base.DbFunction_AddMinutes_Date();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%M', datetime(""o"".""OrderDate"", 7 || ' minute')) AS INTEGER) = 7",
               Sql);
        }

        public override void DbFunction_AddSeconds_Date()
        {
            base.DbFunction_AddSeconds_Date();

            Assert.Contains(
               @"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE CAST(strftime('%S', datetime(""o"".""OrderDate"", 6 || ' second')) AS INTEGER) = 6",
               Sql);
        }

        public override void DbFunction_DiffDays_Date()
        {
            base.DbFunction_DiffDays_Date();

            Assert.Contains(@"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE -(julianday(""o"".""OrderDate"") - julianday(@__Parse_0)) = -7",
              Sql);
        }

        public override void DbFunction_DiffHours_Date()
        {
            base.DbFunction_DiffHours_Date();

            Assert.Contains(@"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE -((julianday(""o"".""OrderDate"") - julianday(@__Parse_0)) * 24) > 0",
              Sql);
        }

        public override void DbFunction_DiffMinutes_Date()
        {
            base.DbFunction_DiffMinutes_Date();

            Assert.Contains(@"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE -((julianday(""o"".""OrderDate"") - julianday(@__Parse_0)) * 1440) > 0",
              Sql);
        }

        public override void DbFunction_DiffSeconds_Date()
        {
            base.DbFunction_DiffSeconds_Date();

            Assert.Contains(@"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE -((julianday(""o"".""OrderDate"") - julianday(@__Parse_0)) * 86400) > 0",
              Sql);
        }

        public override void DbFunction_TruncateTime_Date()
        {
            base.DbFunction_TruncateTime_Date();

            Assert.Contains(@"SELECT ""o"".""OrderID"", ""o"".""CustomerID"", ""o"".""EmployeeID"", ""o"".""OrderDate""
FROM ""Orders"" AS ""o""
WHERE date(""o"".""OrderDate"") = date(@__Parse_0)",
             Sql);
        }

        #endregion

        private const string FileLineEnding = @"
";

        private static string Sql => TestSqlLoggerFactory.Sql.Replace(Environment.NewLine, FileLineEnding);
    }
}
