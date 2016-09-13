using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UserAreaRepository : IUserAreaRepository
    {
        #region constructor

        private readonly Dictionary<string, IUserArea> _userAreas;

        public UserAreaRepository(
            IUserArea[] userAreas
            )
        {
            _userAreas = userAreas
                .Where(k => !(k is CustomEntityDynamicEntityDefinition))
                .ToDictionary(k => k.UserAreaCode);
        }

        #endregion

        /// <summary>
        /// Gets a registered user area by it's code, thowing an exception if
        /// the area does not exist.
        /// </summary>
        public IUserArea GetByCode(string code)
        {
            var area = _userAreas.GetOrDefault(code);

            if (area == null)
            {
                throw new EntityNotFoundException("UserArea not registered: " + code);
            }

            return area;
        }


        public IEnumerable<IUserArea> GetAll()
        {
            return _userAreas.Select(a => a.Value);
        }
    }
}
