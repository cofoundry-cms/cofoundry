using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// Thrown when a child transaction has been rolled back but the primary ITransactionScope
    /// has received a request to complete. Typically code in a child transaction will throw
    /// an exception that will prevent a Complete() call in the parent, but this is a 'catchable'
    /// fall back to prevent the roll back going unoticed.
    /// </summary>
    public class ChildTransactionRolledBackException : Exception
    {
        public ChildTransactionRolledBackException()
        {

        }

        public ChildTransactionRolledBackException(string message)
            : base(message)
        {
        }

        public ChildTransactionRolledBackException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
