using Cofoundry.Core.Data.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data.Internal
{
    /// <summary>
    /// A 'primary' or 'outer' transaction scope that directly represents
    /// the underlying transaction that other child scopes are parented to.
    /// </summary>
    public class PrimaryTransactionScope : ITransactionScope, IDisposable
    {
        private readonly DefaultTransactionScopeManager _transactionScopeManager;
        private Queue<Func<Task>> _runOnCompleteActions = new Queue<Func<Task>>();
        private System.Transactions.TransactionScope _innerScope;

        public PrimaryTransactionScope(
            DefaultTransactionScopeManager transactionScopeManager,
            Func<System.Transactions.TransactionScope> transactionScopeFactory
            )
        {
            _transactionScopeManager = transactionScopeManager;

            _innerScope = transactionScopeFactory();
        }

        /// <summary>
        /// Commits the underlying transaction.
        /// </summary>
        public async Task CompleteAsync()
        {
            _innerScope.Complete();

            // Dispose of the inner scope so transactions are freed up for
            // code running in the on-complete actions
            DisposeInnerScope();

            // Run all actions
            while (_runOnCompleteActions.Count > 0)
            {
                var item = _runOnCompleteActions.Dequeue();
                await item();
            }
        }
        
        public void Dispose()
        {
            DisposeInnerScope();
        }

        private void DisposeInnerScope()
        {
            if (_innerScope != null)
            {
                var scopeToDispose = _innerScope;
                _innerScope = null;

                // De-register this scope as the primary transaction so others can be created
                _transactionScopeManager?.DeregisterTransaction(this);

                // Dispose of the EF transaction which should tidy itself up.
                scopeToDispose.Dispose();
            }
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
            _runOnCompleteActions.Enqueue(actionToQueue);
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
            _runOnCompleteActions.Enqueue(() =>
            {
                actionToQueue();
                return Task.CompletedTask;
            });
        }
    }
}
