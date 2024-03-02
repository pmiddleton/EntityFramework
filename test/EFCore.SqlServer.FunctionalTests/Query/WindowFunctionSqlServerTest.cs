// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class WindowFunctionSqlServerTest : WindowFunctionTestBase<WindowFunctionSqlServerTest.SqlServer>
    {
        public WindowFunctionSqlServerTest(SqlServer fixture, ITestOutputHelper testOutputHelper) : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public class SqlServer : WindowFunctionFixture
        {
            protected override string StoreName => "WindowFunctionTests";

            protected override ITestStoreFactory TestStoreFactory
                => SqlServerTestStoreFactory.Instance;
        }

        #region Tests

        public override void Max_Basic()
        {
            base.Max_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], MAX([e].[Salary]) OVER () AS [MaxSalary]
FROM [Employees] AS [e]
""");
        }

        public override void Max_Null()
        {
            base.Max_Null();

            AssertSql(
                """
SELECT [n].[Id], [n].[Name], MAX([n].[Salary]) OVER () AS [MaxSalary]
FROM [NullTestEmployees] AS [n]
""");
        }

        public override void Min_Basic()
        {
            base.Min_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], MIN([e].[Salary]) OVER () AS [MinSalary]
FROM [Employees] AS [e]
""");
        }

        public override void Min_Null()
        {
            base.Min_Null();

            AssertSql(
                """
SELECT [n].[Id], [n].[Name], MIN([n].[Salary]) OVER () AS [MinSalary]
FROM [NullTestEmployees] AS [n]
""");
        }

        public override void Count_Star_Basic()
        {
            base.Count_Star_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], COUNT(*) OVER () AS [Count]
FROM [Employees] AS [e]
""");
        }

        public override void Count_Col_Basic()
        {
            base.Count_Col_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], COUNT([e].[Id]) OVER () AS [Count]
FROM [Employees] AS [e]
""");
        }

        #endregion

        #region WindowOverExpression Equality tests

        public override void Multiple_Aggregates_Basic_NoDup_Query()
        {
            base.Multiple_Aggregates_Basic_NoDup_Query();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], MAX([e].[Salary]) OVER () AS [MaxSalary], MIN([e].[Salary]) OVER () AS [MinSalary]
FROM [Employees] AS [e]
""");
        }

        public override void Multiple_Aggregates_Basic_Dup_Query()
        {
            base.Multiple_Aggregates_Basic_Dup_Query();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], MAX([e].[Salary]) OVER () AS [MaxSalary1]
FROM [Employees] AS [e]
""");
        }

        #endregion


        public void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
