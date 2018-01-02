using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
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
        //todo - do we need to store this?
        private readonly MethodCallExpression _expression;
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
        public virtual string Name => _expression.Method.Name; //TODO - I need the DBFunction here just use the name for now

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
            //get dbFunction from model
            //temp hack to unwind any expression parameters?
            //var method = expression.Method.DeclaringType.GetMethod(expression.Method.Name, expression.Method.GetParameters()
            //  .Select(p => p.ParameterType == typeof(Expression) ? null : p.ParameterType)
            //.ToArray());
            //hack for test
            var method = expression.Method.DeclaringType.GetMethod(expression.Method.Name, new[] { typeof(int), typeof(int) });

            _dbFunction = model.Relational().FindDbFunction(method);

            //_dbFunction = model.Relational().FindDbFunction(expression.Method);
            
            //todo - what are we going to need here?  The dbFunction, the methodCallExpression, the methodInfo?
            //I need the DbFunction at some point to do the translation.... where am I going to get it from?
            _expression = expression;

            Arguments = _expression.Arguments;

            //todo - need to make sure generic is something other than IQueryable?
            //todo - check return type is a valid type (see dbFunction valid return types)
            //does the IQueryable need to be converted to IEnumerable?
            if (_expression.Method.ReturnType.IsGenericType)
            {
                if (_expression.Method.ReturnType.GetGenericTypeDefinition() != typeof(IQueryable<>))
                {
                    throw new Exception("message here - must be iqueryable");
                }

                Type = _expression.Method.ReturnType;
                ReturnType = _expression.Method.ReturnType.GetGenericArguments()[0];
            }
            else
            {
                Type = typeof(IEnumerable<>).MakeGenericType(_expression.Method.ReturnType);
                ReturnType = _expression.Method.ReturnType;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public DbFunctionSourceExpression([NotNull] DbFunctionSourceExpression oldFuncExpression, [NotNull] ReadOnlyCollection<Expression> newArguments)
        {
            Arguments = new ReadOnlyCollection<Expression>(newArguments);
            _expression = oldFuncExpression._expression;
            _dbFunction = oldFuncExpression._dbFunction;
            ReturnType = oldFuncExpression.ReturnType;
            Type = oldFuncExpression.Type;

            //    OriginalExpression = oldFuncExpression.OriginalExpression;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return base.Accept(visitor);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="visitor">todo</param>
        /// <returns>todo</returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            //  if (!(visitor is TransformingExpressionVisitor) && OriginalExpression != null)
            //    this.OriginalExpression = visitor.Visit(this.OriginalExpression);

            //can the parameterextractingexpressionvisitor unwrap the lambda for me here? - probably not safe
            //can we watch for parameterextractingexpressionvisitor and unwrap ourselves here??
            var newArguments = visitor.Visit(Arguments);

            if (visitor is ParameterExtractingExpressionVisitor)
            {
                newArguments = new ReadOnlyCollection<Expression>(newArguments.Select(a => a is LambdaExpression l ? l.Body : a).ToList());
            }

            return newArguments != Arguments
                ? new DbFunctionSourceExpression(this, newArguments)
                : this;
        }
    }
}
