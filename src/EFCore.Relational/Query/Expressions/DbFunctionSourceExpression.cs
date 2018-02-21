using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class DbFunctionSourceExpression : Expression
    {
        private readonly IDbFunction _dbFunction ;

        /// <summary>
        /// todo
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// todo
        /// </summary>
        public override Type Type { get; }

        /// <summary>
        /// todo
        /// </summary>
        public virtual Type ReturnType { get; }

        /// <summary>
        /// todo
        /// </summary>
        public virtual string Schema => _dbFunction.Schema;

        /// <summary>
        /// todo
        /// </summary>
        public virtual Type UnwrappedType => Type.IsGenericType ? Type.GetGenericArguments()[0] : Type;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string Name => _dbFunction.FunctionName;

        /// <summary>
        /// todo
        /// </summary>
        public virtual bool IsIQueryable => _dbFunction.IsIQueryable;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ReadOnlyCollection<Expression> Arguments { get; [param: NotNull] set; }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="expression">todo</param>
        /// <param name="model">todo</param>
        public DbFunctionSourceExpression([NotNull] MethodCallExpression expression, [NotNull] IModel model)
        {
            _dbFunction = FindDbFunction(expression, model);
            Arguments = expression.Arguments;

            if (expression.Method.ReturnType.IsGenericType)
            {
                //todo - add unit test
                if (expression.Method.ReturnType.GetGenericTypeDefinition() != typeof(IQueryable<>))
                {
                    throw new InvalidOperationException(
                        RelationalStrings.DbFunctionTableValuedFunctionMustReturnIQueryable(_dbFunction.FunctionName));
                }

                //todo - should i be using the dbfunction return type here?  If not do I have to verify the expression return type?
                Type = expression.Method.ReturnType;
                ReturnType = expression.Method.ReturnType.GetGenericArguments()[0];
            }
            else
            {
                Type = typeof(IEnumerable<>).MakeGenericType(expression.Method.ReturnType);
                ReturnType = expression.Method.ReturnType;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public DbFunctionSourceExpression([NotNull] DbFunctionSourceExpression oldFuncExpression, [NotNull] ReadOnlyCollection<Expression> newArguments)
        {
            Arguments = new ReadOnlyCollection<Expression>(newArguments);
            _dbFunction = oldFuncExpression._dbFunction;
            ReturnType = oldFuncExpression.ReturnType;
            Type = oldFuncExpression.Type;
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="visitor">todo</param>
        /// <returns>todo</returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newArguments = visitor.Visit(Arguments);

            if (visitor is ParameterExtractingExpressionVisitor)
            {
                newArguments = new ReadOnlyCollection<Expression>(newArguments.Select(a => a is LambdaExpression l ? l.Body : a).ToList());
            }

            return newArguments != Arguments
                ? new DbFunctionSourceExpression(this, newArguments)
                : this;
        }

        private IDbFunction FindDbFunction(MethodCallExpression exp, IModel model)
        {
            var method = exp.Method.DeclaringType.GetMethod(
                exp.Method.Name,
                exp.Method.GetParameters()
                    .Select(p => UnwrapParamterType(p.ParameterType))
                    .ToArray());

            var dbFunction = model.Relational().FindDbFunction(method);

            //todo - add unit test
            if (dbFunction == null)
            {
                throw new InvalidOperationException(
                    RelationalStrings.DbFunctionNotFound(method.Name));
            }

            return dbFunction;

            Type UnwrapParamterType(Type paramType)
            {
                if (paramType.IsGenericType
                    && paramType.GetGenericTypeDefinition() == typeof(Expression<>))
                {
                    var expressionType = paramType.GetGenericArguments()[0];

                    if (expressionType.IsGenericType
                        && expressionType.GetGenericTypeDefinition() == typeof(Func<>))
                    {
                        return expressionType.GetGenericArguments().Last();
                    }
                }

                return paramType;
            }
        }
    }
}
