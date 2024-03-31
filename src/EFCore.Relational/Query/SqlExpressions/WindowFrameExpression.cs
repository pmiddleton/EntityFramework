// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions;

/// <summary>
/// todo
/// </summary>
public abstract class WindowFrameExpression : Expression, IPrintableExpression
{
    /// <summary>
    /// todo
    /// </summary>
    public SqlExpression? Preceding { get; init; }

    /// <summary>
    /// todo
    /// </summary>
    public SqlExpression? Following { get; init; }

    /// <summary>
    /// todo - bettter name
    /// </summary>
    public abstract string FrameName { get; }

    /// <summary>
    /// todo
    /// </summary>
    public WindowFrameExpression(SqlExpression? preceding, SqlExpression? following)
    {
        //todo - exception if both null?

        Preceding = preceding;
        Following = following;
    }

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
     => this;

    /// <inheritdoc />
    void IPrintableExpression.Print(ExpressionPrinter expressionPrinter)
    {
        //todo
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj != null
            && (ReferenceEquals(this, obj)
                || obj is WindowFrameExpression windowFrameExpression
                && Equals(windowFrameExpression));

    private bool Equals(WindowFrameExpression windowFrameExpression)
        => base.Equals(windowFrameExpression)
            && FrameName == windowFrameExpression.FrameName
            && ((Preceding == null && windowFrameExpression.Preceding == null)
                || (Preceding != null && Preceding.Equals(windowFrameExpression.Preceding)))
            && ((Following == null && windowFrameExpression.Following == null)
                || (Following != null && Following.Equals(windowFrameExpression.Following)));

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(base.GetHashCode());
        hash.Add(FrameName);
        hash.Add(Preceding);
        hash.Add(Following);

        return hash.ToHashCode();
    }
}

/// <summary>
/// todo
/// </summary>
public class WindowFrameRowExpression : WindowFrameExpression
{
    /// <inheritdoc />
    public override string FrameName => "ROWS";

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="preceding">todo</param>
    /// <param name="following">todo</param>
    public WindowFrameRowExpression(SqlExpression? preceding, SqlExpression? following)
        : base(preceding, following)
    {
    }
}

/// <summary>
/// todo
/// </summary>
public class WindowFrameRangeExpression : WindowFrameExpression
{
    /// <inheritdoc />
    public override string FrameName => "RANGE";

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="preceding">todo</param>
    /// <param name="following">todo</param>
    public WindowFrameRangeExpression(SqlExpression? preceding, SqlExpression? following)
        : base(preceding, following)
    {
    }
}


/// <summary>
/// todo
/// </summary>
public class WindowFrameGroupsExpression : WindowFrameExpression
{
    /// <inheritdoc />
    public override string FrameName => "GROUPS";

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="preceding">todo</param>
    /// <param name="following">todo</param>
    public WindowFrameGroupsExpression(SqlExpression? preceding, SqlExpression? following)
        : base(preceding, following)
    {
    }
}
