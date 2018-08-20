using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data
{
    /// <summary>
    /// Abstraction of an database transaction that allows for nesting of child
    /// transactions.
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// Commits the transaction for this scope instance. Whether the underlying
        /// transaction is commited or not depends on whether this is a parent or 
        /// child scope. See concrete implementation for more information.
        /// </summary>
        Task CompleteAsync();

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
        void QueueCompletionTask(Func<Task> actionToQueue);

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
        void QueueCompletionTask(Action actionToQueue);
    }
}
