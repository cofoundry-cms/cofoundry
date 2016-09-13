using Conditions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// An abstraction over EF transactions which allows for nested transaction scopes.
    /// This assumes only 1 transaction per DbContext instance (which is all EF supports).
    /// </summary>
    /// <remarks>
    /// This abstraction was created because System.Transactions is not entirely supported
    /// in EF and I had issues using it. The advice was to use Database.BeginTransaction()
    /// but this does not support nested scopes. 
    /// See http://stackoverflow.com/questions/34159752/nested-transaction-behavior-in-ef6
    /// </remarks>
    public class TransactionScopeFactory : ITransactionScopeFactory
    {
        private readonly DbContext _dbContext;
        private TransactionScope _primaryTransactionScope = null;

        public TransactionScopeFactory(
            DbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new transaction scope. The scope can be nested inside another scope
        /// in which case the underlying db transaction is only commited once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
        /// </summary>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        public ITransactionScope Create()
        {
            ITransactionScope scope;

            if (_primaryTransactionScope == null)
            {
                var transaction = _dbContext.Database.BeginTransaction();
                _primaryTransactionScope = new TransactionScope(this, transaction);
                scope = _primaryTransactionScope;
            }
            else
            {
                scope = new ChildTransactionScope(_primaryTransactionScope);
            }

            return scope;
        }

        /// <summary>
        /// Deregisters the specified scope as the primary transaction scope. This
        /// is called when the primary scope is disposed i.e. at the end of a using 
        /// block, thereofre freeing the factory up for another primary transaction.
        /// </summary>
        /// <param name="scope">The primary transaction scope to deallocate.</param>
        internal void DeregisterTransaction(TransactionScope scope)
        {
            Condition.Requires(scope).IsNotNull();

            if (_primaryTransactionScope == null) return;

            if (_primaryTransactionScope != scope)
            {
                throw new InvalidOperationException("DeregisterTransaction can only be called with the currently active TransactionScope");
            }

            _primaryTransactionScope = null;
        }

        public void Dispose()
        {
            if (_primaryTransactionScope != null)
            {
                _primaryTransactionScope.Dispose();
            }
        }
    }
}
