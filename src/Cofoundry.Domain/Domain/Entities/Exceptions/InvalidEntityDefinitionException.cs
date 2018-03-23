using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Thrown at startup when checking the validity of entity
    /// definitions
    /// </summary>
    public class InvalidEntityDefinitionException : Exception
    {
        public InvalidEntityDefinitionException() { }

        /// <summary>
        /// Consructs a new instance of InvalidntityDefinitionException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="invalidDefinition">The entity definition that caused the exception.</param>
        /// <param name="allCustomEntityDefinitions">Optional collection of all the entity definitions when available.</param>
        public InvalidEntityDefinitionException(
            string message,
            IEntityDefinition invalidDefinition,
            IEnumerable<IEntityDefinition> allCustomEntityDefinitions = null
            )
            : base(message)
        {
            InvalidDefinition = invalidDefinition;
            AllDefinitions = allCustomEntityDefinitions?.ToArray();
        }

        /// <summary>
        /// The entity definition that caused the exception.
        /// </summary>
        public IEntityDefinition InvalidDefinition { get; private set; }

        /// <summary>
        /// Optional collection of all the entity definitions when available.
        /// </summary>
        public IReadOnlyCollection<IEntityDefinition> AllDefinitions { get; private set; }
    }
}
