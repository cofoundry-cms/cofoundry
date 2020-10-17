using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Cofoundry.Core.Data.Internal
{
    /// <summary>
    /// A 'child' or 'inner' transaction scope that takes place within a 
    /// parent transaction scope. Nothing happens when the child scope is 
    /// completed, it is only when the parent scope is completed that the 
    /// underlying transaction is committed
    /// </summary>
    public class ChildTransactionScope : ITransactionScope, IDisposable
    {
        private readonly PrimaryTransactionScope _primaryTransactionScope;
        private TransactionScope _innerScope;

        public ChildTransactionScope(
            PrimaryTransactionScope primaryTransactionScope,
            Func<System.Transactions.TransactionScope> transactionScopeFactory
            )
        {
            _primaryTransactionScope = primaryTransactionScope;
            _innerScope = transactionScopeFactory();
        }

        /// <summary>
        /// Marks this child transaction as complete. The underlying transaction
        /// is not completed until the parent and all child transactions are
        /// completed.
        /// </summary>
        public Task CompleteAsync()
        {
            _innerScope.Complete();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _innerScope.Dispose();
        }

        /// <summary>
        /// <para>
        /// Adds a function to a queue that will be processed once the transaction has
        /// completed. In a nested-transaction scenario the queue is only processed once
        /// the outer primary transaction is complete. This is useful to ensure the correct 
        /// execution of code that should only run after a transaction is fully complete, 
        /// irrespective of any parent transaction scopes. 
        /// </para>
        /// <para>
        /// Cache clearing and message publishing are the two main examples of code that 
        /// needs to be queued to run after a transaction is complete. E.g. if a parent
        /// transaction scope rolls back, the child scope will roll back and any queued 
        /// completion tasks in the child scope should not be executed.
        /// </para>
        /// </summary>
        /// <param name="actionToQueue">
        /// An async action to add to the completion task queue. Actions will be
        /// executed in order once the primary (top level) transaction has been 
        /// completed. All completion tasks are executed outside of the transaction 
        /// scope.
        /// </param>
        public void QueueCompletionTask(Func<Task> actionToQueue)
        {
            _primaryTransactionScope.QueueCompletionTask(actionToQueue);
        }

        /// <summary>
        /// <para>
        /// Adds a function to a queue that will be processed once the transaction has
        /// completed. In a nested-transaction scenario the queue is only processed once
        /// the outer primary transaction is complete. This is useful to ensure the correct 
        /// execution of code that should only run after a transaction is fully complete, 
        /// irrespective of any parent transaction scopes. 
        /// </para>
        /// <para>
        /// Cache clearing and message publishing are the two main examples of code that 
        /// needs to be queued to run after a transaction is complete. E.g. if a parent
        /// transaction scope rolls back, the child scope will roll back and any queued 
        /// completion tasks in the child scope should not be executed.
        /// </para>
        /// </summary>
        /// <param name="actionToQueue">
        /// An action to add to the completion task queue. Actions will be
        /// executed in order once the primary (top level) transaction has been 
        /// completed. All completion tasks are executed outside of the transaction 
        /// scope.
        /// </param>
        public void QueueCompletionTask(Action actionToQueue)
        {
            _primaryTransactionScope.QueueCompletionTask(actionToQueue);
        }
    }
}
