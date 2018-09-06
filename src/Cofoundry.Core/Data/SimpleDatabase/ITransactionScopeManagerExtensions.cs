using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Data.SimpleDatabase
{
    public static class ITransactionScopeManagerExtensions
    {
        /// <summary>
        /// Creates a new transaction scope associated with the connection in use by the
        /// specified IDatabase instance. The scope can be nested inside another scope in which 
        /// case the underlying db transaction is only committed once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
        /// </summary>
        /// <param name="database">
        /// <para>
        /// The IDatabase instance to manage transactions for. Transaction scopes
        /// created by this instance only apply to a single DbConnection, so if you want 
        /// the scope to span multiple IDatabase instances then they must share the same 
        /// connection.
        /// </para>
        /// <para>
        /// You can use the ICofoundryDbConnectionManager to get a reference to the shared 
        /// connection directly.
        /// </para>
        /// </param>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        public static ITransactionScope Create(this ITransactionScopeManager transactionScopeManager, IDatabase database)
        {
            return transactionScopeManager.Create(database.GetDbConnection());
        }
    }
}
