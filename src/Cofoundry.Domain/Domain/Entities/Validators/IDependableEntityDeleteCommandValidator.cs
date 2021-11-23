using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used in commands that delete an entity with a definition that implements 
    /// <see cref="IDependableEntityDefinition"/>, where the entity may be related
    /// to other entities via a non-database-level relation. This validator ensures 
    /// that that any dependencies that would prevents an entity from being deleted
    /// are enforced.
    /// </summary>
    public interface IDependableEntityDeleteCommandValidator
    {
        /// <summary>
        /// Validates that no other entities have a required dependency on the specified
        /// entity via a non-database-level relation (e.g. via unstructured data). If
        /// an undeletable relation is found than a <see cref="RequiredDependencyConstaintViolationException"/>
        /// is thrown.
        /// </summary>
        /// <exception cref="RequiredDependencyConstaintViolationException">Thrown is a required dpendency relation is found.</exception>
        /// <param name="entityDefinitionCode"><see cref="IEntityDefinition.EntityDefinitionCode"/> of the entity type being validated.</param>
        /// <param name="entityId">The databse id of the entity being validated.</param>
        /// <param name="executionContext">The current execution context of the executing command to pass down to any nested queries.</param>
        Task ValidateAsync(string entityDefinitionCode, int entityId, IExecutionContext executionContext);

    }
}
