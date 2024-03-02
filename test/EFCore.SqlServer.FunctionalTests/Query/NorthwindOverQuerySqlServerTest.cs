// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindOverQuerySqlServerTest : NorthwindOverQueryRelationalTestBase<
    NorthwindQuerySqlServerFixture<NoopModelCustomizer>>
{
    public NorthwindOverQuerySqlServerTest(
            NorthwindQuerySqlServerFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override bool CanExecuteQueryString
        => true;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());


    public async override Task Over_Dev(bool async)
    {
        await base.Over_Dev(async);

        AssertSql(
"""

""");
    }

    public async override Task Over_Dev_Group(bool async)
    {
        await base.Over_Dev_Group(async);

        AssertSql(
"""

""");
    }


    private void AssertSql(params string[] expected)
    => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
