using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IUserAreaDefinitionRepository
    {
        /// <summary>
        /// Gets a user area definition using the unique code. Throws an exception
        /// if the user area is not registered.
        /// </summary>
        /// <param name="code">Uniquely identifying user area code.</param>
        IUserAreaDefinition GetByCode(string code);

        /// <summary>
        /// Returns all user areas defitions registered in the system.
        /// </summary>
        IEnumerable<IUserAreaDefinition> GetAll();

        /// <summary>
        /// Returns the default user area, which prefers areas with the IsDefaultAuthSchema
        /// property set to true, falling back to the Cofoundry Admin user area.
        /// </summary>
        IUserAreaDefinition GetDefault();
    }
}
