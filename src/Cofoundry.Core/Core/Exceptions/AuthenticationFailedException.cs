using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Thrown when authentication has failed in a lower level service. Can be caught furthur
    /// up the chain and handled accordingly.
    /// </summary>
    public class AuthenticationFailedException : Exception
    {
        const string DEFAULT_MESSAGE = "Authentication failed.";

        public AuthenticationFailedException() : this(DEFAULT_MESSAGE)
        {
        }
        public AuthenticationFailedException(string message)
            : base(message)
        {
        }
    }
}
