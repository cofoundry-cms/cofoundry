using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UserAreaDefinitionRepository : IUserAreaDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, IUserAreaDefinition> _userAreas;

        public UserAreaDefinitionRepository(
            IEnumerable<IUserAreaDefinition> userAreas
            )
        {
            _userAreas = userAreas
                .Where(k => !(k is CustomEntityDynamicEntityDefinition))
                .ToDictionary(k => k.UserAreaCode);
        }

        #endregion

        /// <summary>
        /// Gets a user area definition using the unique code. Throws an exception
        /// if the user area is not registered.
        /// </summary>
        /// <param name="code">Uniquely identifying user area code.</param>
        public IUserAreaDefinition GetByCode(string code)
        {
            var area = _userAreas.GetOrDefault(code);

            if (area == null)
            {
                throw new EntityNotFoundException("UserArea not registered: " + code);
            }

            return area;
        }

        /// <summary>
        /// Returns all user areas defitions registered in the system.
        /// </summary>
        public IEnumerable<IUserAreaDefinition> GetAll()
        {
            return _userAreas.Select(a => a.Value);
        }
    }
}
