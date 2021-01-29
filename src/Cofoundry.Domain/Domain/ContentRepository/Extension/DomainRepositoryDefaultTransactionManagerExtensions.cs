using Cofoundry.Core.Data;
using Cofoundry.Core.Data.Internal;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Cofoundry.Domain.TransactionManager.Default
{
    public static class DomainRepositoryDefaultTransactionManagerExtensions
    {
        /// <summary>
        /// Creates a new transaction scope associated with the specified connection, 
        /// using the specified transaction configuration options. 
        /// The scope can be nested inside another scope in which case the underlying 
        /// db transaction is only committed once both the outer and inner transaction(s) 
        /// have been committed. The returned ITransactionScope implements IDisposable 
        /// and should be wrapped in a using statement.
        /// </summary>
        /// <param name="dbConnection">
        /// <para>
        /// The DbConnection instance to manage transactions for. Transaction scopes
        /// created by this instance only apply to a single DbConnection, so if you want 
        /// the scope to span additional data access mechanism then they must share the 
        /// same connection.
        /// </para>
        /// <para>
        /// You can use the ICofoundryDbConnectionManager to get a reference to the shared 
        /// connection directly.
        /// </para>
        /// </param>
        /// <param name="transactionScopeOption">This is defaulted to TransactionScopeOption.Required.</param>
        /// <param name="isolationLevel">This is defaulted to IsolationLevel.ReadCommitted.</param>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        public static ITransactionScope CreateScope(
            this IDomainRepositoryTransactionManager domainRepositoryTransactionManager,
            System.Transactions.TransactionScopeOption transactionScopeOption = System.Transactions.TransactionScopeOption.Required,
            System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            )
        {
            var extendable = domainRepositoryTransactionManager.AsExtendableDomainRepositoryTransactionManager();
            var defaultTransactionScopeManager = CastDefaultTransactionScopeManager(extendable.TransactionScopeManager);
            return defaultTransactionScopeManager.Create(extendable.DbContext, transactionScopeOption, isolationLevel);
        }

        private static IDefaultTransactionScopeManager CastDefaultTransactionScopeManager(ITransactionScopeManager transactionScopeManager)
        {
            if (!(transactionScopeManager is IDefaultTransactionScopeManager defaultTransactionScopeManager))
            {
                throw new ArgumentException($"{nameof(transactionScopeManager)} must of type {nameof(IDefaultTransactionScopeManager)} to be used with a method that includes transactionscope options.");
            }

            return defaultTransactionScopeManager;
        }

        /// <summary>
        /// Creates a new transaction scope associated with the specified connection, 
        /// creating the inner scope using the specified factory method. 
        /// The scope can be nested inside another scope in which case the underlying 
        /// db transaction is only committed once both the outer and inner transaction(s) 
        /// have been committed. The returned ITransactionScope implements IDisposable 
        /// and should be wrapped in a using statement.
        /// </summary>
        /// <param name="dbConnection">
        /// <para>
        /// The DbConnection instance to manage transactions for. Transaction scopes
        /// created by this instance only apply to a single DbConnection, so if you want 
        /// the scope to span additional data access mechanism then they must share the 
        /// same connection.
        /// </para>
        /// <para>
        /// You can use the ICofoundryDbConnectionManager to get a reference to the shared 
        /// connection directly.
        /// </para>
        /// </param>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        public static ITransactionScope CreateScope(
            this IDomainRepositoryTransactionManager domainRepositoryTransactionManager,
            Func<System.Transactions.TransactionScope> transactionScopeFactory
            )
        {
            var extendable = domainRepositoryTransactionManager.AsExtendableDomainRepositoryTransactionManager();
            var defaultTransactionScopeManager = CastDefaultTransactionScopeManager(extendable.TransactionScopeManager);

            return defaultTransactionScopeManager.Create(extendable.DbContext, transactionScopeFactory);
        }
    }
}