using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Cofoundry.Core.Data.Internal
{
    /// <summary>
    /// Abstraction over the creation of TransactionScope, so the
    /// implementation is overridable.
    /// </summary>
    public interface ITransactionScopeFactory
    {
        /// <summary>
        /// Creates a new TransactionScope. The default implementation
        /// in Cofoundry uses IsolationLevel.ReadCommitted, TransactionScopeAsyncFlowOption.Enabled
        /// TransactionManager.DefaultTimeout and TransactionScopeOption.Required.
        /// </summary>
        TransactionScope Create();

        /// <summary>
        /// Creates a new TransactionScope using the specified options.
        /// </summary>
        /// <param name="transactionScopeOption">This is usually defaulted to TransactionScopeOption.Required.</param>
        /// <param name="isolationLevel">This is defaulted to IsolationLevel.ReadCommitted.</param>
        TransactionScope Create(
            TransactionScopeOption transactionScopeOption,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
            );
    }
}
