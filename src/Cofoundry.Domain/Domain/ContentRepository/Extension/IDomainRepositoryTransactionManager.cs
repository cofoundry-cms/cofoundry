using Cofoundry.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// DomainRepository wrapper around ITransactionScopeManager.
    /// </summary>
    public interface IDomainRepositoryTransactionManager
    {
        /// <summary>
        /// Creates a new transaction scope associated with the connection in use by the
        /// specified dbContext instance. The scope can be nested inside another scope in 
        /// which case the underlying db transaction is only committed once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
        /// </summary>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        ITransactionScope CreateScope();
    }
}
