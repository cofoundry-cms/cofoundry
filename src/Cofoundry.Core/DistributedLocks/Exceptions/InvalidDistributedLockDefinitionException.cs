using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.DistributedLocks
{
    /// <summary>
    /// Thrown at startup when checking the validity of distributed
    /// lock definitions.
    /// </summary>
    public class InvalidDistributedLockDefinitionException : Exception
    {
        public InvalidDistributedLockDefinitionException() { }

        /// <summary>
        /// Consructs a new instance of InvalidDistributedLockDefinitionException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="invalidDefinition">The definition that caused the exception.</param>
        /// <param name="allDefinitions">Optional collection of all the definitions when available.</param>
        public InvalidDistributedLockDefinitionException(
            string message,
            IDistributedLockDefinition invalidDefinition,
            IEnumerable<IDistributedLockDefinition> allDefinitions = null
            )
            : base(message)
        {
            InvalidDefinition = invalidDefinition;
            AllDefinitions = allDefinitions?.ToArray();
        }

        /// <summary>
        /// The definition that caused the exception.
        /// </summary>
        public IDistributedLockDefinition InvalidDefinition { get; private set; }

        /// <summary>
        /// Optional collection of all the definitions when available.
        /// </summary>
        public IReadOnlyCollection<IDistributedLockDefinition> AllDefinitions { get; private set; }
    }
}
