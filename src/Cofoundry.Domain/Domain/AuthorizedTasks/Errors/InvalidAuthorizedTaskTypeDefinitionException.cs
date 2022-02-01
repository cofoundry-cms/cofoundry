using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Thrown at startup when checking the validity of authorized task type
    /// definitions.
    /// </summary>
    public class InvalidAuthorizedTaskTypeDefinitionException : Exception
    {
        public InvalidAuthorizedTaskTypeDefinitionException() { }

        /// <summary>
        /// Constructs a new <see cref="InvalidAuthorizedTaskTypeDefinitionException"/> instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="invalidDefinition">The authorized task type definition that caused the exception.</param>
        /// <param name="allDefinitions">Optional collection of all the authorized task type definitions when available.</param>
        public InvalidAuthorizedTaskTypeDefinitionException(
            string message,
            IAuthorizedTaskTypeDefinition invalidDefinition,
            IEnumerable<IAuthorizedTaskTypeDefinition> allDefinitions = null
            )
            : base(message)
        {
            InvalidDefinition = invalidDefinition;
            AllDefinitions = allDefinitions?.ToArray();
        }

        /// <summary>
        /// The authorized task type definition that caused the exception.
        /// </summary>
        public IAuthorizedTaskTypeDefinition InvalidDefinition { get; private set; }

        /// <summary>
        /// Optional collection of all the authorized task type definitions when available.
        /// </summary>
        public IReadOnlyCollection<IAuthorizedTaskTypeDefinition> AllDefinitions { get; private set; }
    }
}
