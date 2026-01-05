using Cofoundry.Core.Data;
using Cofoundry.Core.Data.Internal;
using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.TransactionManager.Default;

/// <summary>
/// Extension methods for <see cref="IDomainRepositoryTransactionManager"/>.
/// </summary>
public static class DomainRepositoryDefaultTransactionManagerExtensions
{
    extension(IDomainRepositoryTransactionManager domainRepositoryTransactionManager)
    {
        /// <summary>
        /// Creates a new transaction scope associated with the specified connection, 
        /// using the specified transaction configuration options. 
        /// The scope can be nested inside another scope in which case the underlying 
        /// db transaction is only committed once both the outer and inner transaction(s) 
        /// have been committed. The returned <see cref="ITransactionScope"/> implements
        /// <see cref="IDisposable"/> and should be wrapped in a using statement.
        /// </summary>
        /// <param name="transactionScopeOption">This is defaulted to <see cref="System.Transactions.TransactionScopeOption.Required"/>.</param>
        /// <param name="isolationLevel">This is defaulted to <see cref="System.Transactions.IsolationLevel.ReadCommitted"/>.</param>
        /// <returns>An <see cref="ITransactionScope"/> instance, which is IDisposable and must be disposed.</returns>
        public ITransactionScope CreateScope(
            System.Transactions.TransactionScopeOption transactionScopeOption = System.Transactions.TransactionScopeOption.Required,
            System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadCommitted)
        {
            var extendable = domainRepositoryTransactionManager.AsExtendableDomainRepositoryTransactionManager();
            var defaultTransactionScopeManager = CastDefaultTransactionScopeManager(extendable.TransactionScopeManager);
            return defaultTransactionScopeManager.Create(extendable.DbContext, transactionScopeOption, isolationLevel);
        }

        /// <summary>
        /// Creates a new transaction scope associated with the specified connection, 
        /// creating the inner scope using the specified factory method. 
        /// The scope can be nested inside another scope in which case the underlying 
        /// db transaction is only committed once both the outer and inner transaction(s) 
        /// have been committed. The returned <see cref="ITransactionScope"/> implements
        /// <see cref="IDisposable"/> and should be wrapped in a using statement.
        /// </summary>
        /// <param name="transactionScopeFactory">
        /// Function to use to create and configure a new <see cref="System.Transactions.TransactionScope"/>.
        /// </param>
        /// <returns>An <see cref="ITransactionScope"/> instance, which is IDisposable and must be disposed.</returns>
        public ITransactionScope CreateScope(Func<System.Transactions.TransactionScope> transactionScopeFactory)
        {
            var extendable = domainRepositoryTransactionManager.AsExtendableDomainRepositoryTransactionManager();
            var defaultTransactionScopeManager = CastDefaultTransactionScopeManager(extendable.TransactionScopeManager);

            return defaultTransactionScopeManager.Create(extendable.DbContext, transactionScopeFactory);
        }
    }

    private static IDefaultTransactionScopeManager CastDefaultTransactionScopeManager(ITransactionScopeManager transactionScopeManager)
    {
        if (transactionScopeManager is not IDefaultTransactionScopeManager defaultTransactionScopeManager)
        {
            throw new ArgumentException($"{nameof(transactionScopeManager)} must of type {nameof(IDefaultTransactionScopeManager)} to be used with a method that includes transactionscope options.");
        }

        return defaultTransactionScopeManager;
    }
}
