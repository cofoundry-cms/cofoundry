using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// DomainRepository wrapper around ITransactionScopeManager.
    /// </summary>
    public class DomainRepositoryTransactionManager : IDomainRepositoryTransactionManager
    {
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly CofoundryDbContext _dbContext;

        public DomainRepositoryTransactionManager(
            ITransactionScopeManager transactionScopeManager,
            CofoundryDbContext dbContext
            )
        {
            _transactionScopeManager = transactionScopeManager;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new transaction scope associated with the connection in use by the
        /// specified dbContext instance. The scope can be nested inside another scope in 
        /// which case the underlying db transaction is only committed once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
        /// </summary>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        public ITransactionScope CreateScope()
        {
            return _transactionScopeManager.Create(_dbContext);
        }
    }
}
