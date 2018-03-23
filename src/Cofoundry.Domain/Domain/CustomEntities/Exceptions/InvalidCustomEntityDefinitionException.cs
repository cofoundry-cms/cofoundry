using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Thrown at startup when checking the validity of custom entity
    /// definitions
    /// </summary>
    public class InvalidCustomEntityDefinitionException : Exception
    {
        public InvalidCustomEntityDefinitionException() { }

        /// <summary>
        /// Consructs a new instance of InvalidCustomEntityDefinitionException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="invalidDefinition">The custom entity definition that caused the exception.</param>
        /// <param name="allCustomEntityDefinitions">Optional collection of all the custom entity definitions when available.</param>
        public InvalidCustomEntityDefinitionException(
            string message,
            ICustomEntityDefinition invalidDefinition,
            IEnumerable<ICustomEntityDefinition> allCustomEntityDefinitions = null
            )
            :base(message)
        {
            InvalidDefinition = invalidDefinition;
            AllDefinitions = allCustomEntityDefinitions?.ToArray();
        }

        /// <summary>
        /// The custom entity definition that caused the exception.
        /// </summary>
        public ICustomEntityDefinition InvalidDefinition { get; private set; }

        /// <summary>
        /// Optional collection of all the custom entity definitions when available.
        /// </summary>
        public IReadOnlyCollection<ICustomEntityDefinition> AllDefinitions { get; private set; }
    }
}
