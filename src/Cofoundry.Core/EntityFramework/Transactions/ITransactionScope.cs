using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// Abstraction of an EF database transaction that allows for nesting of child
    /// transactions in a similar way to System.Transactions.TransactionScope.
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// Commits the transaction for this scope instance. Whether the underlying
        /// transaction is commited or not depends on whether this is a parent or 
        /// child scope. See concret implementation for more information.
        /// </summary>
        void Complete();
    }
}
