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
        //private readonly MethodCallExpression _expression;
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
        /// <param name="dbFunction">todo</param>
        /// <param name="arguments">todo</param>
        public DbFunctionExpression([NotNull] IDbFunction dbFunction, ReadOnlyCollection<Expression> arguments)
        {
            //Check.NotNull(expression, nameof(expression));
            Check.NotNull(dbFunction, nameof(dbFunction));

       //     if (!expression.Method.Equals(dbFunction.MethodInfo))
         //   {
         //       throw new Exception("method call db func mismatch");
         //   }

           /* Arguments = new ReadOnlyCollection<Expression>(expression.Arguments.Select<object, Expression>(mp =>
                        {
                            if ((mp as Expression)?.NodeType == ExpressionType.Lambda)
                            { 
                                return Expression.Invoke(mp as Expression);
                            }

                            return Expression.Constant(mp);
                        }).ToList());*/

            Arguments = arguments;
         //   _expression = expression;
            _type = typeof(IEnumerable<>).MakeGenericType(dbFunction.MethodInfo.ReturnType);

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
          //  _expression = oldFuncExpression._expression;
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
    }
}
