using Cofoundry.Core;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A repository to make it easier to get instances of entity 
    /// definitions registered in the DI container.
    /// </summary>
    public interface IEntityDefinitionRepository
    {
        /// <summary>
        /// Returns an entity definition by it's unique <see cref="IEntityDefinition.EntityDefinitionCode"/>. 
        /// If the definition does not exist then <see langword="null"/> is returned.
        /// </summary>
        /// <param name="entityDefinitionCode">The unique 6 character code that identifies the entity definition.</param>
        /// <returns>Returns the matching <see cref="IEntityDefinition"/> instance if one is found; otherwise <see langword="null"/>.</returns>
        IEntityDefinition GetByCode(string entityDefinitionCode);

        /// <summary>
        /// Returns an entity definition by it's unique <see cref="IEntityDefinition.EntityDefinitionCode"/>. 
        /// If the definition does not exist then an <see cref="EntityNotFoundException{IEntityDefinition}"/>
        /// is thrown.
        /// </summary>
        /// <param name="entityDefinitionCode">The unique 6 character code that identifies the entity definition.</param>
        /// <exception cref="EntityNotFoundException{IEntityDefinition}">Thrown if the entity definition could not be found.</exception>
        /// <returns>Returns the matching <see cref="IEntityDefinition"/> instance.</returns>
        IEntityDefinition GetRequiredByCode(string entityDefinitionCode);

        /// <summary>
        /// Returns an entity definition instance by it's type. If the definition 
        /// does not exist then an <see cref="EntityNotFoundException{TDefinition}"/> 
        /// is thrown
        /// </summary>
        /// <typeparam name="TDefinition">The type of definition to find.</typeparam>
        /// <exception cref="EntityNotFoundException{TDefinition}">Thrown if the definition could not be found.</exception>
        /// <returns>Returns the matching <see cref="IEntityDefinition"/> instance.</returns>
        IEntityDefinition GetRequired<TDefinition>() where TDefinition : IEntityDefinition;

        /// <summary>
        /// Returns all entity definitions instances registered in the DI container.
        /// </summary>
        IEnumerable<IEntityDefinition> GetAll();
    }
}