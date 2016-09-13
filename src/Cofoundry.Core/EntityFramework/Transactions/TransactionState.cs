using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// Represents the state of a TransactionScope
    /// </summary>
    internal enum TransactionState
    {
        /// <summary>
        /// Default state, not intended to be used.
        /// </summary>
        Closed,

        /// <summary>
        /// The transaction has started.
        /// </summary>
        Open,

        /// <summary>
        /// The transaction has been committed, can no
        /// longer be rolled back and no futher operation 
        /// can be executed.
        /// </summary>
        Commited,

        /// <summary>
        /// The transaction has been rolled back and no futher operation 
        /// can be executed.
        /// </summary>
        RolledBack
    }
}
