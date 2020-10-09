using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data.Internal
{
    /// <summary>
    /// The default Cofoundry implementation of ITransactionScopeManager 
    /// that uses System.Transaction.TransactionScope internally
    /// to manage transaction scopes. This can be used to cast the injected
    /// ITransactionScopeManager and gain more control over the creation of 
    /// transaction scopes.
    /// </summary>
    public interface IDefaultTransactionScopeManager : ITransactionScopeManager
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
        ITransactionScope Create(DbConnection dbConnection,
                System.Transactions.TransactionScopeOption transactionScopeOption = System.Transactions.TransactionScopeOption.Required,
                System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            );

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
        ITransactionScope Create(DbConnection dbConnection, Func<System.Transactions.TransactionScope> transactionScopeFactory);

        /// <summary>
        /// Creates a new transaction scope associated with the specified connection, 
        /// using the specified transaction configuration options. 
        /// The scope can be nested inside another scope in which case the underlying 
        /// db transaction is only committed once both the outer and inner transaction(s) 
        /// have been committed. The returned ITransactionScope implements IDisposable 
        /// and should be wrapped in a using statement.
        /// </summary>
        /// <param name="dbContext">
        /// <para>
        /// The EF DbContext instance to manage transactions for. Transaction scopes
        /// created by this instance only apply to a single DbConnection, so if you want 
        /// the scope to span multiple contexts then they must share the same connection.
        /// </para>
        /// <para>
        /// If you are using multiple EF DbContexts on the Cofoundry database you can make 
        /// them share the scoped Cofoundry connection by initializing the context with 
        /// ICofoundryDbContextInitializer. Alternatively you can use 
        /// ICofoundryDbConnectionManager to get a reference to the shared connection 
        /// directly.
        /// </para>
        /// </param>
        /// <param name="transactionScopeOption">This is defaulted to TransactionScopeOption.Required.</param>
        /// <param name="isolationLevel">This is defaulted to IsolationLevel.ReadCommitted.</param>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        ITransactionScope Create(DbContext dbContext,
                System.Transactions.TransactionScopeOption transactionScopeOption = System.Transactions.TransactionScopeOption.Required,
                System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            );

        /// <summary>
        /// Creates a new transaction scope associated with the specified connection, 
        /// creating the inner scope using the specified factory method. 
        /// The scope can be nested inside another scope in which case the underlying 
        /// db transaction is only committed once both the outer and inner transaction(s) 
        /// have been committed. The returned ITransactionScope implements IDisposable 
        /// and should be wrapped in a using statement.
        /// </summary>
        /// <param name="dbContext">
        /// <para>
        /// The EF DbContext instance to manage transactions for. Transaction scopes
        /// created by this instance only apply to a single DbConnection, so if you want 
        /// the scope to span multiple contexts then they must share the same connection.
        /// </para>
        /// <para>
        /// If you are using multiple EF DbContexts on the Cofoundry database you can make 
        /// them share the scoped Cofoundry connection by initializing the context with 
        /// ICofoundryDbContextInitializer. Alternatively you can use 
        /// ICofoundryDbConnectionManager to get a reference to the shared connection 
        /// directly.
        /// </para>
        /// </param>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        ITransactionScope Create(DbContext dbContext, Func<System.Transactions.TransactionScope> transactionScopeFactory);
    }
}
