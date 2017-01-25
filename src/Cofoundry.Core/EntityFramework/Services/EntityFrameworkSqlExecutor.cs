using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// A service for executing raw SQL statements agsint an EF DataContext.
    /// </summary>
    public class EntityFrameworkSqlExecutor : IEntityFrameworkSqlExecutor
    {
        #region constructor

        private readonly DbContext _dbContext;
        private readonly ISqlParameterFactory _sqlParameterFactory;

        public EntityFrameworkSqlExecutor(
            DbContext dbContext,
            ISqlParameterFactory sqlParameterFactory
            )
        {
            _dbContext = dbContext;
            _sqlParameterFactory = sqlParameterFactory;
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a stored procedure returning the results as an array forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// An array of the results of the query.
        /// </returns>
        public T[] ExecuteQuery<T>(string spName, params SqlParameter[] sqlParams)
        {
            var results = CreateQuery<T>(spName, sqlParams).ToArray();
            return results;
        }

        /// <summary>
        /// Executes a stored procedure returning the results as an array forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// An array of the results of the query.
        /// </returns>
        public async Task<T[]> ExecuteQueryAsync<T>(string spName, params SqlParameter[] sqlParams)
        {
            var results = await CreateQuery<T>(spName, sqlParams).ToArrayAsync();
            return results;
        }

        private DbRawSqlQuery<T> CreateQuery<T>(string spName, params SqlParameter[] sqlParams)
        {
            if (sqlParams.Any())
            {
                var cmd = FormatSqlCommand(spName, sqlParams);

                var results = _dbContext.Database.SqlQuery<T>(cmd, sqlParams);

                return results;
            }
            else
            {
                return _dbContext.Database.SqlQuery<T>(spName);
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a stored procedure returning a single result and forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The result of the query. Throws an exception if more than one result is returned.
        /// </returns>
        public T ExecuteScalar<T>(string spName, params SqlParameter[] sqlParams)
        {
            var results = CreateQuery<T>(spName, sqlParams).SingleOrDefault();
            return results;
        }

        /// <summary>
        /// Executes a stored procedure returning a single result and forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The result of the query. Throws an exception if more than one result is returned.
        /// </returns>
        public async Task<T> ExecuteScalarAsync<T>(string spName, params SqlParameter[] sqlParams)
        {
            var results = await CreateQuery<T>(spName, sqlParams).SingleOrDefaultAsync();
            return results;
        }


        #endregion

        #region ExecuteCommand

        /// <summary>
        /// Executes a stored procedure or function returning either the number of rows affected or 
        /// optionally returning the value of the first output parameter passed in the parameters collection.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// Either the number of rows affected or optionally returning the value of the 
        /// first output parameter passed int he parameters collection.
        /// </returns>
        public object ExecuteCommand(string spName, params SqlParameter[] sqlParams)
        {
            if (sqlParams.Any())
            {
                var cmd = FormatSqlCommand(spName, sqlParams);
                FormatSqlParameters(sqlParams);

                int rowsAffected = _dbContext.Database.ExecuteSqlCommand(cmd, sqlParams);
                return GetOutputParamValue(sqlParams) ?? rowsAffected;
            }
            else
            {
                return _dbContext.Database.ExecuteSqlCommand(spName);
            }
        }

        /// <summary>
        /// Executes a stored procedure or function returning either the number of rows affected or 
        /// optionally returning the value of the first output parameter passed in the parameters collection.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// Either the number of rows affected or optionally returning the value of the 
        /// first output parameter passed in the parameters collection.
        /// </returns>
        public async Task<object> ExecuteCommandAsync(string spName, params SqlParameter[] sqlParams)
        {
            if (sqlParams.Any())
            {
                var cmd = FormatSqlCommand(spName, sqlParams);
                FormatSqlParameters(sqlParams);

                int rowsAffected = await _dbContext.Database.ExecuteSqlCommandAsync(cmd, sqlParams);
                return GetOutputParamValue(sqlParams) ?? rowsAffected;
            }
            else
            {
                return await _dbContext.Database.ExecuteSqlCommandAsync(spName);
            }
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
        /// as the output parameter type.
        /// </summary>
        /// <typeparam name="T">Type of the returned output parameter.</typeparam>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="outputParameterName">Name to use when creating the output parameter</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The value of the first output parameter in the executed query.
        /// </returns>
        public T ExecuteCommandWithOutput<T>(string spName, string outputParameterName, params SqlParameter[] sqlParams)
        {
            var outputParam = CreateOutputParameter<T>(outputParameterName);
            var modifiedParams = MergeParameters(sqlParams, outputParam);

            ExecuteCommand(spName, modifiedParams);

            return ParseOutputParameter<T>(outputParam);
        }

        /// <summary>
        /// Executes a stored procedure or function returning the value of the 
        /// output paramter. The output parameter is created for you so you do not need
        /// to specify it in the sqlParams collection. If more than one output parameter
        /// is specified only the first is returned. The generic type parameter is used
        /// as the output parameter type.
        /// </summary>
        /// <typeparam name="T">Type of the returned output parameter.</typeparam>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="outputParameterName">Name to use when creating the output parameter</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The value of the first output parameter in the executed query.
        /// </returns>
        public async Task<T> ExecuteCommandWithOutputAsync<T>(string spName, string outputParameterName, params SqlParameter[] sqlParams)
        {
            var outputParam = CreateOutputParameter<T>(outputParameterName);
            var modifiedParams = MergeParameters(sqlParams, outputParam);

            await ExecuteCommandAsync(spName, modifiedParams);

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
