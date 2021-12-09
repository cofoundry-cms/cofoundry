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
        /// If the definition does not exist then <see langword="null"/> is returned.
        /// </summary>
        /// <param name="userAreaCode">The unique 3 character code that identifies the user area definition.</param>
        /// <returns>Returns the matching <see cref="IUserAreaDefinition"/> instance, or <see langword="null"/> if it does not exist.</returns>
        IUserAreaDefinition GetByCode(string userAreaCode);

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
        /// Returns the default user area, which prefers areas with the <see cref="IUserAreaDefinition.IsDefaultAuthScheme"/>
        /// property set to true, falling back to the Cofoundry Admin user area.
        /// </summary>
        IUserAreaDefinition GetDefault();

        /// <summary>
        /// Returns a set of <see cref="UserAreaOptions"/> configured for the
        /// specified user area. Option configuration is layered starting with 
        /// defaults, then layering on global <see cref="IdentitySettings"/> and
        /// finally running <see cref="IUserAreaDefinition.ConfigureOptions"/>.
        /// </summary>
        /// <param name="userAreaCode">The unique 3 character code that identifies the user area definition.</param>
        /// <returns>A configured <see cref="UserAreaOptions"/> instance.</returns>
        UserAreaOptions GetOptionsByCode(string userAreaCode);
    }
}
