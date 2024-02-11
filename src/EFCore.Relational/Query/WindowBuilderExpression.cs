// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions.Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Query
{
    internal class WindowBuilderExpression : Expression
    {
        public List<OrderingExpression> OrderingExpressions { get; } = new List<OrderingExpression>();
        public WindowPartitionExpression? PartitionExpression { get; set; }

        //todo - Rows and Range
    }
}
