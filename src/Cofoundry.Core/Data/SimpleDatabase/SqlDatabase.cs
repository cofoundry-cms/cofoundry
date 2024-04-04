using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Cofoundry.Core.Data.SimpleDatabase.Internal;

/// <summary>
/// Simple MS SqlServer raw sql execution to avoid a dependency on any particular
/// framework.
/// </summary>
public sealed class SqlDatabase : IDisposable, IDatabase
{
    private readonly SqlConnection _sqlConnection;

    public SqlDatabase(SqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    /// <inheritdoc/>
    public DbConnection GetDbConnection()
    {
        return _sqlConnection;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(string sql, params SqlParameter[]? sqlParams)
    {
        ArgumentNullException.ThrowIfNull(sql);

        var isInitialStateClosed = IsClosed();
        if (isInitialStateClosed)
        {
            _sqlConnection.Open();
        }

        using (var sqlCmd = new SqlCommand(sql, _sqlConnection))
        {
            if (sqlParams != null && sqlParams.Length != 0)
            {
                sqlCmd.Parameters.AddRange(sqlParams);
            }
            await sqlCmd.ExecuteNonQueryAsync();
        }

        if (isInitialStateClosed)
        {
            _sqlConnection.Close();
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<TEntity>> ReadAsync<TEntity>(
        string sql,
        Func<SqlDataReader, TEntity> mapper,
        params SqlParameter[]? sqlParams
        )
    {
        ArgumentNullException.ThrowIfNull(sql);
        ArgumentNullException.ThrowIfNull(mapper);

        var isInitialStateClosed = IsClosed();
        if (isInitialStateClosed)
        {
            _sqlConnection.Open();
        }

        var result = new List<TEntity>();

        using (var sqlCmd = new SqlCommand(sql, _sqlConnection))
        {
            if (sqlParams != null && sqlParams.Length != 0)
            {
                sqlCmd.Parameters.AddRange(sqlParams);
            }

            using (var reader = await sqlCmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var mapped = mapper(reader);
                    result.Add(mapped);
                }
            }
        }

        if (isInitialStateClosed)
        {
            _sqlConnection.Close();
        }

        return result;
    }

    private bool IsClosed()
    {
        return _sqlConnection.State == ConnectionState.Closed;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_sqlConnection != null && _sqlConnection.State != ConnectionState.Closed)
        {
            _sqlConnection.Close();
        }
    }
}
