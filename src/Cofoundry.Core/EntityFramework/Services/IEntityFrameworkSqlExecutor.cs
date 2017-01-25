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
    /// A service for executing raw SQL statements against an EF DataContext.
    /// </summary>
    public interface IEntityFrameworkSqlExecutor
    {
        #region ExecuteQuery

        /// <summary>
        /// Executes a stored procedure returning the results as an array forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// An array of the results of the query.
        /// </returns>
        T[] ExecuteQuery<T>(string spName, params SqlParameter[] sqlParams);

        /// <summary>
        /// Executes a stored procedure returning the results as an array forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// An array of the results of the query.
        /// </returns>
        Task<T[]> ExecuteQueryAsync<T>(string spName, params SqlParameter[] sqlParams);

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
        T ExecuteScalar<T>(string spName, params SqlParameter[] sqlParams);

        /// <summary>
        /// Executes a stored procedure returning a single result and forcing query execution.
        /// </summary>
        /// <param name="spName">Name of the stored procedure to run</param>
        /// <param name="sqlParams">Collection of SqlParameters to pass to the command</param>
        /// <returns>
        /// The result of the query. Throws an exception if more than one result is returned.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(string spName, params SqlParameter[] sqlParams);

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
        object ExecuteCommand(string spName, params SqlParameter[] sqlParams);

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
        Task<object> ExecuteCommandAsync(string spName, params SqlParameter[] sqlParams);

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
        T ExecuteCommandWithOutput<T>(string spName, string outputParameterName, params SqlParameter[] sqlParams);

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
        Task<T> ExecuteCommandWithOutputAsync<T>(string spName, string outputParameterName, params SqlParameter[] sqlParams);

        #endregion
    }
}
