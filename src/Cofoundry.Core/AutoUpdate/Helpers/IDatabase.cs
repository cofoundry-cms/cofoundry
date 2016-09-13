using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Simple Db raw sql execution to avoid a dependency on any particular
    /// framework
    /// </summary>
    public interface IDatabase
    {
        IDatabaseTransaction BeginTransaction();
        void Execute(string sql, params SqlParameter[] sqlParams);
        IEnumerable<TEntity> Read<TEntity>(string sql, Func<SqlDataReader, TEntity> map, params SqlParameter[] sqlParams);
    }
}
