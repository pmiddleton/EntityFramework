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

        #region Base Window Functions Tests

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

        public override void RowNumber_Basic()
        {
            base.RowNumber_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], ROW_NUMBER() OVER ( ORDER BY [e].[Name]) AS [RowNumber]
FROM [Employees] AS [e]
""");
        }

        public override void First_Value_OderByEnd_Basic()
        {
            base.First_Value_OderByEnd_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], FIRST_VALUE([e].[Name]) OVER ( ORDER BY [e].[Salary]) AS [FirstValue]
FROM [Employees] AS [e]
""");
        }

        public override void First_Value_FrameEnd_Basic()
        {
            base.First_Value_FrameEnd_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], FIRST_VALUE([e].[Name]) OVER ( ORDER BY [e].[Salary] ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS [FirstValue]
FROM [Employees] AS [e]
""");
        }

        public override void First_Value_Null()
        {
            base.First_Value_Null();

            AssertSql(
                """
SELECT [n].[Id], [n].[Name], FIRST_VALUE([n].[Salary]) OVER ( ORDER BY [n].[WorkExperience]) AS [FirstValue]
FROM [NullTestEmployees] AS [n]
""");
        }

        public override void Last_Value_OderByEnd_Basic()
        {
            base.Last_Value_OderByEnd_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], LAST_VALUE([e].[Name]) OVER ( ORDER BY [e].[Salary]) AS [LastValue]
FROM [Employees] AS [e]
""");
        }

        public override void Last_Value_FrameEnd_Basic()
        {
            base.Last_Value_FrameEnd_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], LAST_VALUE([e].[Name]) OVER ( ORDER BY [e].[Salary] ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS [LastValue]
FROM [Employees] AS [e]
""");
        }

        public override void Last_Value_Null()
        {
            base.Last_Value_Null();

            AssertSql(
                """
SELECT [n].[Id], [n].[Name], LAST_VALUE([n].[Salary]) OVER ( ORDER BY [n].[WorkExperience]) AS [LastValue]
FROM [NullTestEmployees] AS [n]
""");
        }

        public override void Rank_Basic()
        {
            base.Rank_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], RANK() OVER (PARTITION BY [e].[DepartmentName] ORDER BY [e].[WorkExperience]) AS [Rank]
FROM [Employees] AS [e]
""");
        }

        public override void Dense_Rank_Basic()
        {
            base.Dense_Rank_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], DENSE_RANK() OVER (PARTITION BY [e].[DepartmentName] ORDER BY [e].[WorkExperience]) AS [Rank]
FROM [Employees] AS [e]
""");
        }

        public override void NTile_Basic()
        {
            base.NTile_Basic();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], NTILE(3) OVER (PARTITION BY [e].[DepartmentName] ORDER BY [e].[WorkExperience]) AS [Rank]
FROM [Employees] AS [e]
""");
        }

        public override void Avg_Decimal()
        {
            base.Avg_Decimal();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], AVG([e].[Salary]) OVER () AS [AverageSalary]
FROM [Employees] AS [e]
""");
        }

        public override void Avg_Int()
        {
            base.Avg_Int();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], AVG([e].[WorkExperience]) OVER () AS [AverageWork]
FROM [Employees] AS [e]
""");
        }

        public override void Avg_Decimal_Int_Cast_Decimal()
        {
            base.Avg_Decimal_Int_Cast_Decimal();

            AssertSql(
                """
SELECT [e].[Id], [e].[Name], AVG(CAST([e].[WorkExperience] AS decimal(18,2))) OVER () AS [AverageWork]
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

        #endregion

        


        public void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
