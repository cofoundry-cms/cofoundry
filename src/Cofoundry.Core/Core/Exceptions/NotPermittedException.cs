using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Thrown when a resource is attempted to be accessed by a user who does not have 
    /// sufficient permissions to do so. Can be caught furthur up the chain and handled accordingly.
    /// </summary>
    public class NotPermittedException : Exception
    {
        public NotPermittedException()
        {
        }
        public NotPermittedException(string message)
            : base(message)
        {
        }
    }
}
