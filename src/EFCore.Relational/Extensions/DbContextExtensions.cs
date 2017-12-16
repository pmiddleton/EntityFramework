using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                        .GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Expression)}, null));

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

            var exp = new DbFunctionExpression(dbFunc, methodCallExp.Arguments);

            var resultsQuery = _dbFuncExecute(dbContext, exp) as IEnumerable<T>;

            var results = resultsQuery.ToList();

            //todo - verify there are results here :)
            return results[0];
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="T">todo</typeparam>
        /// <param name="dbContext">todo</param>
        /// <param name="methodInfo">todo</param>
        /// <param name="methodParams">todo</param>
        /// <returns>todo</returns>
        public static T ExecuteScalarMethod<T>(this DbContext dbContext, MethodInfo methodInfo, object[] methodParams)
        {
            var dbFunc = dbContext.Model.Relational().FindDbFunction(methodInfo);

            if (dbFunc == null)
            {
                throw new Exception("cant find dbFunc");
            }

            var exp = new DbFunctionExpression(dbFunc, new ReadOnlyCollection<Expression>(methodParams.Select(p => p is Expression ? (Expression)p : Expression.Constant(p)).ToList()));

            var resultsQuery = _dbFuncExecute(dbContext, exp) as IEnumerable<T>;

            var results = resultsQuery.ToList();

            //todo - verify there are results here :)
            return results[0];
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <typeparam name="T">todo</typeparam>
        /// <param name="dbContext">todo</param>
        /// <param name="methodParams">todo</param>
        /// <param name="callerName">todo</param>
        /// <returns>todo</returns>
        public static T ExecuteScalarMethod<T>(this DbContext dbContext, object[] methodParams, [CallerMemberName] string callerName = "")
        {
            var paramTypes = methodParams.Select(mp => (mp as LambdaExpression)?.ReturnType ?? mp.GetType()).ToArray();

            //TODO - test and  clean this up
            var methodInfo = dbContext.GetType().GetTypeInfo().GetDeclaredMethods(callerName)
                .SingleOrDefault(mi =>
                {
                    var miParams = mi.GetParameters();

                    if (paramTypes.Length == miParams.Length)
                    {
                        for (var i = 0; i < paramTypes.Length; i++)
                        {
                            if (paramTypes[i] != miParams[i].ParameterType)
                            { 
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
                });

            return dbContext.ExecuteScalarMethod<T>(methodInfo, methodParams);
        }
    }
}
