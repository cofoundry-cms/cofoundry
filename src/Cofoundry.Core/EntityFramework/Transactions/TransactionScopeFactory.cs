using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private Dictionary<DbContext, TransactionScope> _primaryTransactionScopes = new Dictionary<DbContext, TransactionScope>();

        /// <summary>
        /// Creates a new transaction scope. The scope can be nested inside another scope
        /// in which case the underlying db transaction is only commited once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
        /// </summary>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        public ITransactionScope Create(DbContext dbContext)
        {
            ITransactionScope scope;
            var primaryScope = _primaryTransactionScopes.GetOrDefault(dbContext);

            if (primaryScope == null)
            {
                primaryScope = CreateScope(dbContext);
                _primaryTransactionScopes.Add(dbContext, primaryScope);
                scope = primaryScope;
            }
            else
            {
                scope = new ChildTransactionScope(primaryScope);
            }

            return scope;
        }

        private TransactionScope CreateScope(DbContext dbContext)
        {
            var transaction = dbContext.Database.BeginTransaction();
            var scope = new TransactionScope(this, transaction);

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
            if (scope == null) throw new ArgumentNullException(nameof(scope));

            var dbContextToRemoveScopeFor = _primaryTransactionScopes
                .Where(s => s.Value == scope)
                .Select(s => s.Key)
                .SingleOrDefault();

            if (dbContextToRemoveScopeFor == null) return;

            _primaryTransactionScopes.Remove(dbContextToRemoveScopeFor);
        }

        public void Dispose()
        {
            foreach (var scope in _primaryTransactionScopes)
            {
                scope.Value.Dispose();
            }
            _primaryTransactionScopes.Clear();
        }
    }
}
