using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Cofoundry.Core.Data.Internal
{
    public class TransactionScopeFactory : ITransactionScopeFactory
    {
        public TransactionScope Create()
        {
            return Create(
                TransactionScopeOption.Required,
                IsolationLevel.ReadCommitted
                );
        }

        public TransactionScope Create(
            TransactionScopeOption transactionScopeOption,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
            )
        {
            var options = new TransactionOptions()
            {
                IsolationLevel = isolationLevel,
                Timeout = TransactionManager.DefaultTimeout
            };

            return new TransactionScope(
                transactionScopeOption,
                options,
                TransactionScopeAsyncFlowOption.Enabled
                );
        }
    }
}
