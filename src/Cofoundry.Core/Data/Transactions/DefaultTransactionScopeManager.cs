using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Cofoundry.Core.Data.Internal
{
    /// <summary>
    /// <para>
    /// A transaction management abstraction which allows for nested transaction scopes
    /// and queuable tasks to execute after the primary (top-level) transaction scope has 
    /// completed. Transactions are limited to one per connection, so primary scopes and 
    /// nesting are based on the connection passed in when creating the scope. 
    /// </para>
    /// <para>
    /// To ensure your database code uses a single transaction scope, ensure you are using 
    /// the same connection for all your data access. If you are using multiple EF 
    /// DbContexts on the Cofoundry database you can make them share the scoped Cofoundry 
    /// connection by initializing the context with ICofoundryDbContextInitializer.
    /// Alternatively you can use ICofoundryDbConnectionManager to get a reference to
    /// the shared connection directly.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This default implementation uses System.Transaction.TransactionScope internally
    /// to manage transaction scopes. Prior to .NET Core 2.0 this used EF transactions
    /// because System.Transactions was not available.
    /// </remarks>
    public class DefaultTransactionScopeManager : IDefaultTransactionScopeManager
    {
        private Dictionary<int, PrimaryTransactionScope> _primaryTransactionScopes = new Dictionary<int, PrimaryTransactionScope>();
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public DefaultTransactionScopeManager(
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        /// <summary>
        /// Creates a new transaction scope associated with the specified connection. 
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
        public ITransactionScope Create(DbConnection dbConnection)
        {
            return Create(dbConnection, CreateScopeFactory());
        }

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
        public ITransactionScope Create(DbConnection dbConnection,
                System.Transactions.TransactionScopeOption transactionScopeOption = System.Transactions.TransactionScopeOption.Required,
                System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            )
        {
            var transactionScopeFactory = CreateScopeFactory(transactionScopeOption, isolationLevel);
            return Create(dbConnection, transactionScopeFactory);
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
        public ITransactionScope Create(DbConnection dbConnection, Func<System.Transactions.TransactionScope> transactionScopeFactory)
        {
            if (dbConnection == null) throw new ArgumentNullException(nameof(dbConnection));
            if (transactionScopeFactory == null) throw new ArgumentNullException(nameof(transactionScopeFactory));

            ITransactionScope scope;
            var connectionHash = dbConnection.GetHashCode();
            var primaryScope = _primaryTransactionScopes.GetOrDefault(connectionHash);

            if (primaryScope == null)
            {
                primaryScope = new PrimaryTransactionScope(this, transactionScopeFactory);
                _primaryTransactionScopes.Add(connectionHash, primaryScope);
                scope = primaryScope;
            }
            else
            {
                scope = new ChildTransactionScope(primaryScope, transactionScopeFactory);
            }

            return scope;
        }

        /// <summary>
        /// Creates a new transaction scope associated with the connection in use by the
        /// specified dbContext instance. The scope can be nested inside another scope in 
        /// which case the underlying db transaction is only committed once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
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
        public ITransactionScope Create(DbContext dbContext)
        {
            var connection = dbContext.Database.GetDbConnection();

            return Create(connection);
        }

        public ITransactionScope Create(DbContext dbContext, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var connection = dbContext.Database.GetDbConnection();

            return Create(connection, transactionScopeOption, isolationLevel);
        }

        public ITransactionScope Create(DbContext dbContext, Func<TransactionScope> transactionScopeFactory)
        {
            var connection = dbContext.Database.GetDbConnection();

            return Create(connection, transactionScopeFactory);
        }

        /// <summary>
        /// <para>
        /// Adds a function to a queue that will be processed once the ambient primary 
        /// transaction scope has completed, if there is no primary transaction scope then 
        /// the function is executed immediately. In a nested-transaction scenario the 
        /// queue is only processed once the outer primary transaction is complete. This 
        /// is useful to ensure the correct execution of code that should only run after a 
        /// transaction is fully complete, irrespective of any parent transaction scopes. 
        /// </para>
        /// <para>
        /// Cache clearing and message publishing are the two main examples of code that 
        /// needs to be queued to run after a transaction is complete. E.g. if a parent
        /// transaction scope rolls back, the child scope will roll back and any queued 
        /// completion tasks in the child scope should not be executed.
        /// </para>
        /// </summary>
        /// <param name="dbConnection">
        /// The dbConnection instance you expect might be taking part in an ambient 
        /// transaction.
        /// </param>
        /// <param name="actionToQueue">
        /// An action to add to the completion task queue. Actions will be
        /// processed in order once the primary (top level) transaction scope has been 
        /// completed or immediately if there is no primary transaction scope. All 
        /// completion tasks are executed outside of the transaction scope.
        /// </param>
        public void QueueCompletionTask(DbConnection dbConnection, Action actionToQueue)
        {
            var connectionHash = dbConnection.GetHashCode();
            var scope = _primaryTransactionScopes.GetOrDefault(connectionHash);

            // No scope, execute immediately
            if (scope == null)
            {
                actionToQueue();
            }
            else
            {
                scope.QueueCompletionTask(actionToQueue);
            }
        }

        /// <summary>
        /// <para>
        /// Adds a function to a queue that will be processed once the ambient primary 
        /// transaction scope has completed, if there is no primary transaction scope then 
        /// the function is executed immediately. In a nested-transaction scenario the 
        /// queue is only processed once the outer primary transaction is complete. This 
        /// is useful to ensure the correct execution of code that should only run after a 
        /// transaction is fully complete, irrespective of any parent transaction scopes. 
        /// </para>
        /// <para>
        /// Cache clearing and message publishing are the two main examples of code that 
        /// needs to be queued to run after a transaction is complete. E.g. if a parent
        /// transaction scope rolls back, the child scope will roll back and any queued 
        /// completion tasks in the child scope should not be executed.
        /// </para>
        /// </summary>
        /// <param name="dbContext">
        /// The EF DbContext instance you expect might be taking part in an ambient 
        /// transaction.
        /// </param>
        /// <param name="actionToQueue">
        /// An async action to add to the completion task queue. Actions will be
        /// processed in order once the primary (top level) transaction scope has been 
        /// completed or immediately if there is no primary transaction scope. All 
        /// completion tasks are executed outside of the transaction scope.
        /// </param>
        public Task QueueCompletionTaskAsync(DbConnection dbConnection, Func<Task> actionToQueue)
        {
            var connectionHash = dbConnection.GetHashCode();
            var scope = _primaryTransactionScopes.GetOrDefault(connectionHash);

            // No scope, execute immediately
            if (scope == null) return actionToQueue();

            scope.QueueCompletionTask(actionToQueue);

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// <para>
        /// Adds a function to a queue that will be processed once the ambient primary 
        /// transaction scope has completed, if there is no primary transaction scope then 
        /// the function is executed immediately. In a nested-transaction scenario the 
        /// queue is only processed once the outer primary transaction is complete. This 
        /// is useful to ensure the correct execution of code that should only run after a 
        /// transaction is fully complete, irrespective of any parent transaction scopes. 
        /// </para>
        /// <para>
        /// Cache clearing and message publishing are the two main examples of code that 
        /// needs to be queued to run after a transaction is complete. E.g. if a parent
        /// transaction scope rolls back, the child scope will roll back and any queued 
        /// completion tasks in the child scope should not be executed.
        /// </para>
        /// </summary>
        /// <param name="dbContext">
        /// The EF DbContext instance you expect might be taking part in an ambient 
        /// transaction.
        /// </param>
        /// <param name="actionToQueue">
        /// An action to add to the completion task queue. Actions will be
        /// processed in order once the primary (top level) transaction scope has been 
        /// completed or immediately if there is no primary transaction scope. All 
        /// completion tasks are executed outside of the transaction scope.
        /// </param>
        public void QueueCompletionTask(DbContext dbContext, Action actionToQueue)
        {
            var dbConnection = dbContext.Database.GetDbConnection();
            QueueCompletionTask(dbConnection, actionToQueue);
        }

        /// <summary>
        /// <para>
        /// Adds a function to a queue that will be processed once the ambient primary 
        /// transaction scope has completed, if there is no primary transaction scope then 
        /// the function is executed immediately. In a nested-transaction scenario the 
        /// queue is only processed once the outer primary transaction is complete. This 
        /// is useful to ensure the correct execution of code that should only run after a 
        /// transaction is fully complete, irrespective of any parent transaction scopes. 
        /// </para>
        /// <para>
        /// Cache clearing and message publishing are the two main examples of code that 
        /// needs to be queued to run after a transaction is complete. E.g. if a parent
        /// transaction scope rolls back, the child scope will roll back and any queued 
        /// completion tasks in the child scope should not be executed.
        /// </para>
        /// </summary>
        /// <param name="dbContext">
        /// The EF DbContext instance you expect might be taking part in an ambient 
        /// transaction.
        /// </param>
        /// <param name="actionToQueue">
        /// An async action to add to the completion task queue. Actions will be
        /// processed in order once the primary (top level) transaction scope has been 
        /// completed or immediately if there is no primary transaction scope. All 
        /// completion tasks are executed outside of the transaction scope.
        /// </param>
        public Task QueueCompletionTaskAsync(DbContext dbContext, Func<Task> actionToQueue)
        {
            var dbConnection = dbContext.Database.GetDbConnection();
            return QueueCompletionTaskAsync(dbConnection, actionToQueue);
        }

        internal void DeregisterTransaction(PrimaryTransactionScope scope)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));

            var scopeToRemoveKey = _primaryTransactionScopes
                .Where(s => s.Value == scope)
                .Select(s => (int?)s.Key)
                .SingleOrDefault();

            if (scopeToRemoveKey.HasValue)
            {
                _primaryTransactionScopes.Remove(scopeToRemoveKey.Value);
            }
        }

        private Func<System.Transactions.TransactionScope> CreateScopeFactory(
                System.Transactions.TransactionScopeOption transactionScopeOption = System.Transactions.TransactionScopeOption.Required,
                System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            )
        {
            return () => _transactionScopeFactory.Create(
                transactionScopeOption,
                isolationLevel
            );
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
