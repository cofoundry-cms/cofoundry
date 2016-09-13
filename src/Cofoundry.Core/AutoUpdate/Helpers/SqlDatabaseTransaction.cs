using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Simple transaction abstraction for sqlserver db
    /// framework
    /// </summary>
    public class SqlDatabaseTransaction : IDatabaseTransaction
    {
        public SqlDatabaseTransaction(
            SqlTransaction transaction
            )
        {
            Transaction = transaction;
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public SqlTransaction Transaction { get; private set; }

        public bool IsClosed { get; private set; }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
                Transaction = null;
                IsClosed = true;
            }
        }
    }
}
