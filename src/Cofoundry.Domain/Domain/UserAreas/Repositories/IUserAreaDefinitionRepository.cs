using Cofoundry.Core;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A repository to make it easier to get instances of user area 
    /// definitions registered in the DI container.
    /// </summary>
    public interface IUserAreaDefinitionRepository
    {
        /// <summary>
        /// Returns a user area definition by it's unique <see cref="IUserAreaDefinition.UserAreaCode"/>. 
        /// If the definition does not exist then an <see cref="EntityNotFoundException{IUserAreaDefinition}"/>
        /// is thrown.
        /// </summary>
        /// <param name="userAreaCode">The unique 3 character code that identifies the user area definition.</param>
        /// <exception cref="EntityNotFoundException{IUserAreaDefinition}">Thrown if the user area definition could not be found.</exception>
        /// <returns>Returns the matching <see cref="IUserAreaDefinition"/> instance.</returns>
        IUserAreaDefinition GetRequiredByCode(string userAreaCode);

        /// <summary>
        /// Returns all user area definitions instances registered in the DI container.
        /// </summary>
        IEnumerable<IUserAreaDefinition> GetAll();

        /// <summary>
        /// Returns the default user area, which prefers areas with the <see cref="IUserAreaDefinition.IsDefaultAuthSchema"/>
        /// property set to true, falling back to the Cofoundry Admin user area.
        /// </summary>
        IUserAreaDefinition GetDefault();
    }
}
