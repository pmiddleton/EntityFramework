using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// todo
    /// </summary>
    public static class DbContextExtensions
    {
        private static Func<DbContext, Expression, object> _dbFuncExecute = (Func<DbContext, Expression, object>)
                Delegate.CreateDelegate(
                    typeof(Func<DbContext, Expression, object>),
                    typeof(DbContext)
                        .GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Expression)}, null));

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="U">todo</typeparam>
        /// <typeparam name="T">todo</typeparam>
        /// <param name="dbContext">todo</param>
        /// <param name="dbFuncCall">todo</param>
        /// <returns>todo</returns>
        public static T ExecuteScalarMethod<U, T>(this DbContext dbContext, [NotNull]Expression<Func<U, T>> dbFuncCall)
            where U : DbContext
        {
            Check.NotNull(dbFuncCall, nameof(dbFuncCall));

            if (!(dbFuncCall.Body is MethodCallExpression methodCallExp))
            {
                throw new Exception("must be method call");
            }

            var dbFunc = dbContext.Model.Relational().FindDbFunction(methodCallExp.Method);

            if (dbFunc == null)
            {
                throw new Exception("cant find dbFunc");
            }

            var exp = new DbFunctionExpression(methodCallExp, dbFunc);

            var resultsQuery = _dbFuncExecute(dbContext, exp) as IEnumerable<T>;

            var results = resultsQuery.ToList();

            //todo - verify there are results here :)
            return results[0];
        }
    }
}
