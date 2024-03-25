using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Cofoundry.Core.Data.SimpleDatabase.Internal;

/// <summary>
/// Simple Db raw sql execution on the Cofoundry database which by
/// default used the shared Cofoundry connection with a scoped lifetime. 
/// This can be used in place of an EF DbContext to avoid a dependency on 
/// any particular framework. Does not support connection resiliency 
/// or retry logic.
/// </summary>
public sealed class CofoundrySqlDatabase : IDisposable, ICofoundryDatabase
{
    private readonly SqlDatabase _sqlDatabase;

    public CofoundrySqlDatabase(
        ICofoundryDbConnectionManager _cofoundryDbConnectionFactory
        )
    {
        var dbConnection = _cofoundryDbConnectionFactory.GetShared();

        if (dbConnection is SqlConnection sqlConnection)
        {
            _sqlDatabase = new SqlDatabase(sqlConnection);
        }
        else
        {
            throw new NotSupportedException($"{nameof(SqlDatabase)} only supports SqlConnection. {nameof(ICofoundryDbConnectionManager)} returned connection of type {dbConnection.GetType().FullName}");
        }
    }

    /// <summary>
    /// Returns the database connection being used by this instance. Used
    /// to enlist in transactions.
    /// </summary>
    public DbConnection GetDbConnection()
    {
        return _sqlDatabase.GetDbConnection();
    }

    /// <summary>
    /// Executes a sql command with the specified parameters.
    /// </summary>
    /// <param name="sql">Raw SQL string to execute against the database.</param>
    /// <param name="sqlParams">Any parameters to add to the command.</param>
    public Task ExecuteAsync(string sql, params SqlParameter[] sqlParams)
    {
        return _sqlDatabase.ExecuteAsync(sql, sqlParams);
    }

    /// <summary>
    /// Executes raw sql and uses a reader with a mapping function to return
    /// typed results.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity returned from the query.</typeparam>
    /// <param name="sql">The raw sql to execute against the database.</param>
    /// <param name="map">A mapping function to use to map each row.</param>
    /// <param name="sqlParams">Any parameters to add to the command.</param>
    /// <returns>Collection of mapped entities.</returns>
    public Task<IReadOnlyCollection<TEntity>> ReadAsync<TEntity>(
        string sql,
        Func<SqlDataReader, TEntity> mapper,
        params SqlParameter[] sqlParams
        )
    {

        return _sqlDatabase.ReadAsync(sql, mapper, sqlParams);
    }

    public void Dispose()
    {
        _sqlDatabase?.Dispose();
    }
}
