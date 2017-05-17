using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Simple Db raw sql execution to avoid a dependency on any particular
    /// framework
    /// </summary>
    public class Database : IDisposable, IDatabase
    {
        private readonly SqlConnection _sqlConnection;
        private SqlDatabaseTransaction _databaseTransaction;
        private static readonly object _lock = new object();

        public Database(
            DatabaseSettings databaseSettings
            )
        {
            _sqlConnection = new SqlConnection(databaseSettings.ConnectionString);
        }

        public IDatabaseTransaction BeginTransaction()
        {
            if (IsTransactionClosed())
            {
                lock (_lock)
                {
                    if (IsTransactionClosed())
                    {
                        var transaction = _sqlConnection.BeginTransaction();
                        _databaseTransaction = new SqlDatabaseTransaction(transaction);
                    }
                }
            }

            return _databaseTransaction;
        }

        public void Execute(string sql, params SqlParameter[] sqlParams)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            using (var sqlCmd = CreateCommand(sql))
            {
                if (sqlParams != null && sqlParams.Any())
                {
                    sqlCmd.Parameters.AddRange(sqlParams);
                }
                sqlCmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<TEntity> Read<TEntity>(string sql, Func<SqlDataReader, TEntity> map, params SqlParameter[] sqlParams)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
            {
                _sqlConnection.Open();
            }

            using (var sqlCmd = new SqlCommand(sql, _sqlConnection))
            {
                if (sqlParams != null && sqlParams.Any())
                {
                    sqlCmd.Parameters.AddRange(sqlParams);
                }

                using (var reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return map(reader);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                _sqlConnection.Dispose();
            }
        }

        #region private helpers

        private SqlCommand CreateCommand(string sql)
        {
            if (IsTransactionClosed())
            {
                return new SqlCommand(sql, _sqlConnection);
            }
            else
            {
                return new SqlCommand(sql, _sqlConnection, _databaseTransaction.Transaction);
            }
        }

        private bool IsTransactionClosed()
        {
            return _databaseTransaction == null || _databaseTransaction.IsClosed;
        }

        #endregion
    }
}
