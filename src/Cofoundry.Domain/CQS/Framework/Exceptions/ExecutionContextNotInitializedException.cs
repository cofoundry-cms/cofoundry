using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// An exception for when the ExecutionContext supplied to a handler is not valid
    /// because data has not been initialized correctly.
    /// </summary>
    public class ExecutionContextNotInitializedException : Exception
    {
        private const string errorMessage = "The ExecutionContext was not initialized correctly.";

        public ExecutionContextNotInitializedException()
        {
        }

        public ExecutionContextNotInitializedException(string message)
            : base(message)
        {
        }
    }
}
