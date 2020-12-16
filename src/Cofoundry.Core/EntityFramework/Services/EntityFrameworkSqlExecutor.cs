using Cofoundry.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace Cofoundry.Core.EntityFramework.Internal
{
    /// <summary>
    /// A service for executing raw SQL statements against an EF DataContext.
    /// </summary>
    public class EntityFrameworkSqlExecutor : IEntityFrameworkSqlExecutor
    {
        #region constructor

        private readonly ISqlParameterFactory _sqlParameterFactory;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public EntityFrameworkSqlExecutor(
            ISqlParameterFactory sqlParameterFactory,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _sqlParameterFactory = sqlParameterFactory;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a stored procedure returning the results as an array forcing query execution.
        /// </summary>
        /// <param name="dbContext">EF DbContext to run the command against.</param>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// An array of the results of the query.
        /// </returns>
        public async virtual Task<T[]> ExecuteQueryAsync<T>(DbContext dbContext, string spName, params SqlParameter[] sqlParams)
            where T : class
        {
            var results = await CreateQuery<T>(dbContext, spName, sqlParams).ToArrayAsync();
            return results;
        }

        private IQueryable<T> CreateQuery<T>(DbContext dbContext, string spName, params SqlParameter[] sqlParams)
            where T : class
        {
            if (sqlParams.Any())
            {
                var cmd = FormatSqlCommand(spName, sqlParams);

                // Here we used to use SqlQuery() in EF6 but it isn't supported
                // until raw sql access is added to EF we'll have to be restricted 
                // to entities only.
                // see https://github.com/aspnet/EntityFramework/issues/1862
                // and https://github.com/aspnet/EntityFrameworkCore/issues/10753

                var results = dbContext.Set<T>().FromSqlRaw(cmd, sqlParams);

                return results;
            }
            else
            {
                return dbContext.Set<T>().FromSqlRaw(spName);
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a stored procedure returning a single result and forcing query 
        /// execution. This does not run in a transaction.
        /// </summary>
        /// <param name="dbContext">EF DbContext to run the command against.</param>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The result of the query. Throws an exception if more than one result is returned.
        /// </returns>
        public async virtual Task<T> ExecuteScalarAsync<T>(DbContext dbContext, string spName, params SqlParameter[] sqlParams)
        {
            // Derived from code in https://github.com/dotnet/efcore/issues/1862
            // Needs to be updated when EF Core finally supports ad-hoc queries again
            var databaseFacade = dbContext.Database;
            var concurrencyDetector = databaseFacade.GetService<IConcurrencyDetector>();
            var sql = FormatSqlCommand(spName, sqlParams);

            using (concurrencyDetector.EnterCriticalSection())
            {
                var rawSqlCommand = databaseFacade
                    .GetService<IRawSqlCommandBuilder>()
                    .Build(sql, sqlParams);

                var commandParameters = new RelationalCommandParameterObject(
                    databaseFacade.GetService<IRelationalConnection>(),
                    rawSqlCommand.ParameterValues,
                    null,
                    dbContext,
                    null);

                var result = await rawSqlCommand
                    .RelationalCommand
                    .ExecuteScalarAsync(commandParameters);

                var typedResult = ParseScalarResult<T>(result);

                return typedResult;
            }
        }

        private T ParseScalarResult<T>(object result)
        {
            if (result == DBNull.Value) return default(T);

            // If this is a non-null value nullable type, return the converted base type
            var nullableType = Nullable.GetUnderlyingType(typeof(T));
            if (nullableType != null)
            {
                return (T)Convert.ChangeType(result, nullableType);
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }

        #endregion

        #region ExecuteCommand

        /// <summary>
        /// Executes a stored procedure or function returning either the number of rows affected or 
        /// optionally returning the value of the first output parameter passed in the parameters 
        /// collection. The command is executed in a transaction created via ITransactionFactory.
        /// </summary>
        /// <param name="dbContext">EF DbContext to run the command against.</param>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// Either the number of rows affected or optionally returning the value of the 
        /// first output parameter passed in the parameters collection.
        /// </returns>
        public async virtual Task<object> ExecuteCommandAsync(DbContext dbContext, string spName, params SqlParameter[] sqlParams)
        {
            object result = null;

            if (sqlParams.Any())
            {
                var cmd = FormatSqlCommand(spName, sqlParams);
                FormatSqlParameters(sqlParams);
                int rowsAffected = 0;

                await dbContext.Database.CreateExecutionStrategy().Execute(async () =>
                {
                    using (var scope = _transactionScopeFactory.Create(dbContext))
                    {
                        rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(cmd, sqlParams);
                        await scope.CompleteAsync();
                    }
                });

                result = GetOutputParamValue(sqlParams) ?? rowsAffected;
            }
            else
            {
                await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var scope = _transactionScopeFactory.Create(dbContext))
                    {
                        result = await dbContext.Database.ExecuteSqlRawAsync(spName);
                        await scope.CompleteAsync();
                    }
                });
            }

            return result;
        }

        private static object GetOutputParamValue(SqlParameter[] sqlParams)
        {
            var outputParam = sqlParams.FirstOrDefault(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output);
            if (outputParam != null)
            {
                return outputParam.Value;
            }
            return null;
        }

        #endregion

        #region ExecuteCommandWithOutput

        /// <summary>
        /// Executes a stored procedure or function returning the value of the 
        /// output paramter. The output parameter is created for you so you do not need
        /// to specify it in the sqlParams collection. If more than one output parameter
        /// is specified only the first is returned. The generic type parameter is used
        /// as the output parameter type. The command is executed in a transaction created 
        /// via ITransactionFactory.
        /// </summary>
        /// <typeparam name="T">Type of the returned output parameter.</typeparam>
        /// <param name="dbContext">EF DbContext to run the command against.</param>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="outputParameterName">Name to use when creating the output parameter</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The value of the first output parameter in the executed query.
        /// </returns>
        public async virtual Task<T> ExecuteCommandWithOutputAsync<T>(DbContext dbContext, string spName, string outputParameterName, params SqlParameter[] sqlParams)
        {
            var outputParam = CreateOutputParameter<T>(outputParameterName);
            var modifiedParams = MergeParameters(sqlParams, outputParam);

            await ExecuteCommandAsync(dbContext, spName, modifiedParams);

            return ParseOutputParameter<T>(outputParam);
        }

        private SqlParameter[] MergeParameters(SqlParameter[] sqlParams, SqlParameter paramToMerge)
        {
            var modifiedParams = sqlParams
                .Union(new SqlParameter[] { paramToMerge })
                .ToArray();

            return modifiedParams;
        }

        private T ParseOutputParameter<T>(SqlParameter outputParam)
        {
            if (outputParam.Value == DBNull.Value) return default(T);

            // If this is a non-null value nullable type, return the converted base type
            var nullableType = Nullable.GetUnderlyingType(typeof(T));
            if (nullableType != null)
            {
                return (T)Convert.ChangeType(outputParam.Value, nullableType);
            }
            return (T)Convert.ChangeType(outputParam.Value, typeof(T));
        }

        private SqlParameter CreateOutputParameter<T>(string outputParameterName)
        {
            var outputParam = _sqlParameterFactory.CreateOutputParameterByType(outputParameterName, typeof(T));


            return outputParam;
        }

        #endregion

        #region private helpers

        private void FormatSqlParameters(SqlParameter[] sqlParams)
        {
            foreach (var xmlParam in sqlParams)
            {
                if (xmlParam.Value == null)
                {
                    // Nulls need to be DbNull
                    xmlParam.Value = DBNull.Value;
                }
                else if (xmlParam.Value is XElement)
                {
                    // We need to re-map xml params because EF doesn't seem to like it
                    xmlParam.SqlDbType = SqlDbType.Xml;
                    xmlParam.Value = xmlParam.Value.ToString();
                }
            }
        }

        private string FormatSqlCommand(string spName, SqlParameter[] sqlParams)
        {
            var formattedParams = sqlParams
                .Select(p => string.Format("@{0} {1}", p.ParameterName.Trim('@'), GetParameterDirection(p)).TrimEnd())
                .ToArray();
            var cmd = spName + " " + string.Join(", ", formattedParams);
            return cmd;
        }

        private string GetParameterDirection(SqlParameter p)
        {
            if (p.Direction == ParameterDirection.InputOutput
                || p.Direction == ParameterDirection.Output)
            {
                return "out";
            }

            return string.Empty;
        }

        #endregion
    }
}
