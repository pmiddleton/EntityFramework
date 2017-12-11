using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    /// todo
    /// </summary>
    public class DbFunctionExpression : Expression
    {
        //todo - do we need to store this?
        private readonly MethodCallExpression _expression;
        private readonly Type _type;

        /// <summary>
        /// todo
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// todo
        /// </summary>
        public override Type Type => _type;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string Name => DbFunction.FunctionName;

        /// <summary>
        /// todo
        /// </summary>
        public virtual IDbFunction DbFunction { get; }

        /// <summary>
        /// todo
        /// </summary>
        public virtual ReadOnlyCollection<Expression> Arguments { get; private set; }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="expression">todo</param>
        /// <param name="dbFunction">todo</param>
        public DbFunctionExpression([NotNull] MethodCallExpression expression, [NotNull] IDbFunction dbFunction)
        {
          //  Check.NotNull(expression, nameof(expression));
           // Check.NotNull(dbFunction, nameof(dbFunction));

            //if (!expression.Method.Equals(dbFunction.MethodInfo))
           // {
           //     throw new Exception("method call db func mismatch");
           // }

            Arguments = expression.Arguments;
            _expression = expression;
            _type = typeof(IEnumerable<>).MakeGenericType(expression.Method.ReturnType);

            TreeWalker t = new TreeWalker();
            t.Visit(expression.Arguments[0]);
            
            DbFunction = dbFunction;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public DbFunctionExpression([NotNull] DbFunctionExpression oldFuncExpression, [NotNull] ReadOnlyCollection<Expression> newArguments)
        {
            Arguments = new ReadOnlyCollection<Expression>(newArguments);
            DbFunction = oldFuncExpression.DbFunction;
            _expression = oldFuncExpression._expression;
            _type = oldFuncExpression._type;
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
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var oldArguments = Arguments;

            var newArguments = visitor.VisitAndConvert(oldArguments, $"{nameof(DbFunctionExpression)}.{nameof(VisitChildren)}");

            return oldArguments != newArguments ? new DbFunctionExpression(this, newArguments) : this;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used 
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate([NotNull] ReadOnlyCollection<Expression> arguments)
        {
            return DbFunction.Translation != null ? DbFunction.Translation(arguments) : null;
        }

        private class TreeWalker : ExpressionVisitor
        {
            public override Expression Visit(Expression node)
            {
                return base.Visit(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Type == typeof(Expression<>))
                {
                    if (node.Type.GetGenericArguments()?[0].Name == "Func`")
                    {
                        int i = 5;
                        i++;
                    }
                }

                if (node.Expression != null)
                { 
                    return base.Visit(node.Expression);


                }
                else
                { 
                    return base.VisitMember(node);
                }
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Value is Expression exp)
                {
                    return base.Visit(exp);
                }
                else
                { 
                    return base.VisitConstant(node);
                }
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return base.VisitLambda(node);
            }

            protected override Expression VisitExtension(Expression node)
            {
                return base.VisitExtension(node);
            }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                return base.VisitRuntimeVariables(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(node);
            }
        }
    }
}
