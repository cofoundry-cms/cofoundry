using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to RoleMicroSummary objects.
    /// </summary>
    public class RoleMicroSummaryMapper : IRoleMicroSummaryMapper
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public RoleMicroSummaryMapper(
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        /// <summary>
        /// Maps an EF Role record from the db into an RoleDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbRole">Role record from the database.</param>
        public virtual RoleMicroSummary Map(Role dbRole)
        {
            if (dbRole == null) return null;

            var role = new RoleMicroSummary()
            {
                RoleId = dbRole.RoleId,
                Title = dbRole.Title
            };

            var userArea = _userAreaRepository.GetByCode(dbRole.UserAreaCode);
            role.UserArea = new UserAreaMicroSummary()
            {
                UserAreaCode = dbRole.UserAreaCode,
                Name = userArea.Name
            };

            return role;
        }
    }
}
