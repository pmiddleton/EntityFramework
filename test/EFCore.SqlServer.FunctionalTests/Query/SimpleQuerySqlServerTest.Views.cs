// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Query
{
    public partial class SimpleQuerySqlServerTest
    {
        public override void View_simple()
        {
            base.View_simple();

            AssertSql(
                @"SELECT [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle]
FROM [Customers] AS [c]");
        }

        public override void View_where_simple()
        {
            base.View_where_simple();

            AssertSql(
                @"SELECT [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle]
FROM [Customers] AS [c]
WHERE [c].[City] = N'London'");
        }

        public override void View_backed_by_view()
        {
            base.View_backed_by_view();

            AssertSql(
                @"SELECT [a].[CategoryName], [a].[ProductID], [a].[ProductName]
FROM [Alphabetical list of products] AS [a]");
        }

        public override void View_with_mixed_tracking()
        {
            base.View_with_mixed_tracking();

            AssertSql(
                @"SELECT [c].[CustomerID], [c].[Address], [c].[City], [c].[CompanyName], [c].[ContactName], [c].[ContactTitle], [c].[Country], [c].[Fax], [c].[Phone], [c].[PostalCode], [c].[Region], [ov].[CustomerID]
FROM [Customers] AS [c]
CROSS JOIN [Orders] AS [ov]
WHERE [ov].[CustomerID] = [c].[CustomerID]");
        }

        public override void View_with_nav()
        {
            base.View_with_nav();

            AssertSql(
                @"SELECT [ov].[CustomerID]
FROM [Orders] AS [ov]
WHERE [ov].[CustomerID] = N'ALFKI'");
        }

        public override void View_with_included_nav()
        {
            base.View_with_included_nav();

            AssertSql(
                @"SELECT [ov].[CustomerID], [ov.Customer].[CustomerID], [ov.Customer].[Address], [ov.Customer].[City], [ov.Customer].[CompanyName], [ov.Customer].[ContactName], [ov.Customer].[ContactTitle], [ov.Customer].[Country], [ov.Customer].[Fax], [ov.Customer].[Phone], [ov.Customer].[PostalCode], [ov.Customer].[Region]
FROM [Orders] AS [ov]
LEFT JOIN [Customers] AS [ov.Customer] ON [ov].[CustomerID] = [ov.Customer].[CustomerID]
WHERE [ov].[CustomerID] = N'ALFKI'");
        }

        public override void View_with_included_navs_multi_level()
        {
            base.View_with_included_navs_multi_level();

            AssertSql(
                @"SELECT [ov].[CustomerID], [ov.Customer].[CustomerID], [ov.Customer].[Address], [ov.Customer].[City], [ov.Customer].[CompanyName], [ov.Customer].[ContactName], [ov.Customer].[ContactTitle], [ov.Customer].[Country], [ov.Customer].[Fax], [ov.Customer].[Phone], [ov.Customer].[PostalCode], [ov.Customer].[Region]
FROM [Orders] AS [ov]
LEFT JOIN [Customers] AS [ov.Customer] ON [ov].[CustomerID] = [ov.Customer].[CustomerID]
WHERE [ov].[CustomerID] = N'ALFKI'
ORDER BY [ov.Customer].[CustomerID]",
                //
                @"SELECT [ov.Customer.Orders].[OrderID], [ov.Customer.Orders].[CustomerID], [ov.Customer.Orders].[EmployeeID], [ov.Customer.Orders].[OrderDate]
FROM [Orders] AS [ov.Customer.Orders]
INNER JOIN (
    SELECT DISTINCT [ov.Customer0].[CustomerID]
    FROM [Orders] AS [ov0]
    LEFT JOIN [Customers] AS [ov.Customer0] ON [ov0].[CustomerID] = [ov.Customer0].[CustomerID]
    WHERE [ov0].[CustomerID] = N'ALFKI'
) AS [t] ON [ov.Customer.Orders].[CustomerID] = [t].[CustomerID]
ORDER BY [t].[CustomerID]");
        }

        public override void View_select_where_navigation()
        {
            base.View_select_where_navigation();

            AssertSql(
                @"SELECT [ov].[CustomerID]
FROM [Orders] AS [ov]
LEFT JOIN [Customers] AS [ov.Customer] ON [ov].[CustomerID] = [ov.Customer].[CustomerID]
WHERE [ov.Customer].[City] = N'Seattle'");
        }

        public override void View_select_where_navigation_multi_level()
        {
            base.View_select_where_navigation_multi_level();

            AssertSql(
                @"SELECT [ov].[CustomerID]
FROM [Orders] AS [ov]
LEFT JOIN [Customers] AS [ov.Customer] ON [ov].[CustomerID] = [ov.Customer].[CustomerID]
WHERE EXISTS (
    SELECT 1
    FROM [Orders] AS [o]
    WHERE [ov.Customer].[CustomerID] = [o].[CustomerID])");
        }
    }
}
