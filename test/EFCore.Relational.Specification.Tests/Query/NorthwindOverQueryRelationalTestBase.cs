// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.Query;

public abstract class NorthwindOverQueryRelationalTestBase<TFixture> : QueryTestBase<TFixture>
    where TFixture : NorthwindQueryFixtureBase<NoopModelCustomizer>, new()
{
    protected NorthwindOverQueryRelationalTestBase(TFixture fixture)
        : base(fixture)
    {
    }

    protected virtual bool CanExecuteQueryString
        => false;

    protected override QueryAsserter CreateQueryAsserter(TFixture fixture)
        => new RelationalQueryAsserter(
            fixture, RewriteExpectedQueryExpression, RewriteServerQueryExpression, canExecuteQueryString: CanExecuteQueryString);


    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Over_Dev(bool async)
       => AssertQuery(
            async,
            ss => ss.Set<OrderDetail>().Select(od => new
            {
                //od.ProductID,
                //  MaxDiscount = od.Over(od => od.Max(od => od.Discount), od => new { od.ProductID, od.OrderID }),

                // MaxDiscount = od.Over2(agg => agg.Max(thingy => thingy.Discount)),
                //    MaxDiscount2 = od.Over2(agg => agg.Max(od => od.Discount))

                //MaxDiscount3 = WindowFunctionsExtensions.Over3(WindowFunctionsExtensions.Max2(od.Discount), new { od.ProductID, od.OrderID } /* order by */)

                /*                MaxDiscount4 = WindowFunctionsExtensions.Over4(od, WindowFunctionsExtensions.Max2(od.Discount), b => b.OrderBy(s => s.Discount).Select(s => new { od.ProductID, od.OrderID })),*/

                //    MaxDiscount5 = WindowFunctionsExtensions.Over5(od, WindowFunctionsExtensions.Max2(od.Discount), new { od.ProductID, od.OrderID }, b => b.OrderBy(s => s.Discount)),

                //MaxDiscount5 = WindowFunctionsExtensions.Over5Debug(od, b => b.OrderBy(s => s.Discount))


                //MaxDiscount6 = WindowFunctionsExtensions.Over().PartitionBy(od.ProductID, od.OrderID).Max(od.Discount),

                //MaxDiscount7 = WindowFunctionsExtensions.Over().PartitionBy(od.ProductID, od.OrderID).OrderBy(od.OrderID).ThenBy(od.ProductID).ThenBy(od.UnitPrice).Max(od.Discount),

                //     MaxDiscount8 = WindowFunctionsExtensions.Over().OrderBy(od.OrderID).Max(od.Discount),

                //MaxDiscount9 = WindowFunctionsExtensions.Over().OrderBy(od.OrderID).Rows(RowsPreceding.CurrentRow, 6).Max(od.Discount),

                //MaxDiscount10 = WindowFunctionsExtensions.Over().Filter(() => od.Discount == 5).OrderBy(od.OrderID).Rows(5).Exclude(Exclude.NoOthers).Max(od.Discount),

                //MaxDiscount10 = WindowFunctionsExtensions.Over().Filter(() => od.Discount == 5).OrderBy(od.OrderID).Rows(5).Max(od.Discount),

                MaxDiscount11 = WindowFunctionsExtensions.Over().PartitionBy(od.ProductID / 10, od.OrderID).OrderBy(od.OrderID).Rows(RowsPreceding.CurrentRow, 5).Max(od.Discount),
            }));


    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual Task Over_Dev_Group(bool async)
      => AssertQuery(
           async,
           ss => ss.Set<OrderDetail>().GroupBy(od => od.ProductID).Select(g => new
           {

               //    od.ProductID
               //   MaxDiscount = od.Over(od => od.Max(od => od.Discount), od => new { od.ProductID, od.OrderID })
               //MaxDiscount = od.Over2(agg => agg.Max(thingy => thingy.Discount))
               MaxDiscount = g.Max(thingy => thingy.Discount)
           }));
}
