using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    ///     Represents a SQL Table Valued Fuction.
    /// </summary>
    public class TableValuedSqlFunctionExpression : TableExpressionBase
    {
        private SqlFunctionExpression _sqlFunctionExpression;

        /// <summary>
        /// todo
        /// </summary>
        public virtual SqlFunctionExpression SqlFunctionExpression => _sqlFunctionExpression;

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="sqlFunction">todo</param>
        /// <param name="querySource">todo</param>
        /// <param name="alias">todo</param>
        public TableValuedSqlFunctionExpression([NotNull] SqlFunctionExpression sqlFunction, [NotNull] IQuerySource querySource, [CanBeNull] string alias)
             : this(sqlFunction.FunctionName, sqlFunction.Type, sqlFunction.Schema, sqlFunction.Arguments, querySource, alias)
        {

        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="functionName">todo</param>
        /// <param name="returnType">todo</param>
        /// <param name="schema">todo</param>
        /// <param name="arguments">todo</param>
        /// <param name="querySource">todo</param>
        /// <param name="alias">todo</param>
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

        /// <summary>
        /// todo
        /// </summary>
        /// <returns>todo</returns>
        public override string ToString()
        {
            return _sqlFunctionExpression.ToString();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="visitor">todo</param>
        /// <returns>todo</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            return visitor is ISqlExpressionVisitor specificVisitor
                ? specificVisitor.VisitTableValuedSqlFunctionExpression(this)
                : base.Accept(visitor);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="visitor">todo</param>
        /// <returns>todo</returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newArguments = visitor.Visit(new ReadOnlyCollection<Expression>(_sqlFunctionExpression.Arguments.ToList()));

            return newArguments != _sqlFunctionExpression.Arguments
                ? new TableValuedSqlFunctionExpression(new SqlFunctionExpression(_sqlFunctionExpression.FunctionName, Type, _sqlFunctionExpression.Schema, newArguments), QuerySource, Alias)
                : this;
        }
    }
}
