using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Exception thrown when a cycle of dependencies are discovered and
    /// the opration cannot be recovered.
    /// </summary>
    public class CyclicDependencyException : Exception
    {
        public CyclicDependencyException()
            : base(CreateMessage())
        {
        }

        public CyclicDependencyException(string message)
            : base(CreateMessage(message))
        {
        }

        public CyclicDependencyException(string message, Exception exception)
            : base(CreateMessage(message), exception)
        {
        }

        private static string CreateMessage(string message = null)
        {
            const string DEFAULT_MESSAGE = "Cyclic dependency detected.";

            if (string.IsNullOrWhiteSpace(message)) return DEFAULT_MESSAGE;
            return message;
        }
    }
}
