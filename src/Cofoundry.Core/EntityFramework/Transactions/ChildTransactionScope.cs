using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// A 'child' or 'inner' transaction scope that takes place within a 
    /// parent transaction scope. Nothing happens when the child scope is 
    /// commited, it is only when the parent scope is completed that the 
    /// underlying transaction is committed
    /// </summary>
    public class ChildTransactionScope : ITransactionScope, IDisposable
    {
        private readonly TransactionScope _primaryTransactionScope;
        private bool _isComplete = false;

        public ChildTransactionScope(
            TransactionScope primaryTransactionScope
            )
        {
            _primaryTransactionScope = primaryTransactionScope;
        }

        /// <summary>
        /// Marks this child transaction as complete. The underlying transaction
        /// is not completed until the parent and all child transactions are
        /// completed.
        /// </summary>
        public void Complete()
        {
            _isComplete = true;
        }

        public void Dispose()
        {
            if (!_isComplete)
            {
                // Automatically roll back the parent transaction.
                _primaryTransactionScope.Rollback();
            }
        }
    }
}
