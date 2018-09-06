using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data
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
    /// to manage transaction scopes.
    /// </remarks>
    public interface ITransactionScopeManager : IDisposable
    {
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
        ITransactionScope Create(DbConnection dbConnection);

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
        ITransactionScope Create(DbContext dbContext);

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
        void QueueCompletionTask(DbConnection dbConnection, Action actionToQueue);

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
        Task QueueCompletionTaskAsync(DbConnection dbConnection, Func<Task> actionToQueue);

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
        void QueueCompletionTask(DbContext dbContext, Action actionToQueue);

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
        Task QueueCompletionTaskAsync(DbContext dbContext, Func<Task> actionToQueue);

    }
}
