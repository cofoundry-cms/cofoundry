using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Thrown when checking the validity of role
    /// definitions.
    /// </summary>
    public class InvalidRoleDefinitionException : Exception
    {
        public InvalidRoleDefinitionException() { }

        /// <summary>
        /// Consructs a new instance of InvalidRoleDefinitionException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="invalidDefinition">The role definition that caused the exception.</param>
        /// <param name="allDefinitions">Optional collection of all the role definitions when available.</param>
        public InvalidRoleDefinitionException(
            string message,
            IRoleDefinition invalidDefinition,
            IEnumerable<IRoleDefinition> allDefinitions = null
            )
            : base(message)
        {
            InvalidDefinition = invalidDefinition;
            AllDefinitions = allDefinitions?.ToArray();
        }

        /// <summary>
        /// The role definition that caused the exception.
        /// </summary>
        public IRoleDefinition InvalidDefinition { get; private set; }

        /// <summary>
        /// Optional collection of all the role definitions when available.
        /// </summary>
        public IReadOnlyCollection<IRoleDefinition> AllDefinitions { get; private set; }
    }
}
