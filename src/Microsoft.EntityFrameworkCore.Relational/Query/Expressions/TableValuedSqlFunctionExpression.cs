using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    ///     Represents a SQL Table Valued Fuction.
    /// </summary>
    public class TableValuedSqlFunctionExpression : TableExpressionBase
    {
        private SqlFunctionExpression _sqlFunctionExpression;

        public virtual SqlFunctionExpression SqlFunctionExpression => _sqlFunctionExpression;

        public TableValuedSqlFunctionExpression([NotNull] SqlFunctionExpression sqlFunction, [NotNull] IQuerySource querySource, [CanBeNull] string alias)
             : this(sqlFunction.FunctionName, sqlFunction.Type, sqlFunction.Schema, sqlFunction.Arguments, querySource, alias)
        {

        }

        public TableValuedSqlFunctionExpression([NotNull] string functionName,
                [NotNull] Type returnType,
                [CanBeNull] string schema,
                [NotNull] IEnumerable<Expression> arguments,
                [NotNull] IQuerySource querySource,
                [CanBeNull]string alias)
            : base(querySource, alias)
        {
            //TODO - make sure return type is of type IQueryable<T>
            //TODO - Do I even need this class or can I just use the SqlFunctionExpression?  Thus far not much is happening in here
            _sqlFunctionExpression = new SqlFunctionExpression(functionName, returnType, schema, arguments);
        }

        public override string ToString()
        {
            return _sqlFunctionExpression.ToString();
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            //return visitor.Visit(_sqlFunctionExpression);

            Check.NotNull(visitor, nameof(visitor));

            var specificVisitor = visitor as ISqlExpressionVisitor;

            return specificVisitor != null
                ? specificVisitor.VisitTableValuedSqlFunction(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newArguments = visitor.Visit(new ReadOnlyCollection<Expression>(_sqlFunctionExpression.Arguments.ToList()));

            return newArguments != _sqlFunctionExpression.Arguments
                ? new TableValuedSqlFunctionExpression(new SqlFunctionExpression(_sqlFunctionExpression.FunctionName, Type, _sqlFunctionExpression.Schema, newArguments), QuerySource, Alias)
                : this;
        }
    }
}
