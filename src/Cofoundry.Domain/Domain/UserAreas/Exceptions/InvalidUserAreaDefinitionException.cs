using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Thrown at startup when checking the validity of user area
    /// definitions.
    /// </summary>
    public class InvalidUserAreaDefinitionException : Exception
    {
        public InvalidUserAreaDefinitionException() { }

        /// <summary>
        /// Consructs a new instance of InvalidUserAreaDefinitionException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="invalidDefinition">The user area definition that caused the exception.</param>
        /// <param name="allDefinitions">Optional collection of all the user area definitions when available.</param>
        public InvalidUserAreaDefinitionException(
            string message,
            IUserAreaDefinition invalidDefinition,
            IEnumerable<IUserAreaDefinition> allDefinitions = null
            )
            : base(message)
        {
            InvalidDefinition = invalidDefinition;
            AllDefinitions = allDefinitions?.ToArray();
        }

        /// <summary>
        /// The user area definition that caused the exception.
        /// </summary>
        public IUserAreaDefinition InvalidDefinition { get; private set; }

        /// <summary>
        /// Optional collection of all the user area definitions when available.
        /// </summary>
        public IReadOnlyCollection<IUserAreaDefinition> AllDefinitions { get; private set; }
    }
}
