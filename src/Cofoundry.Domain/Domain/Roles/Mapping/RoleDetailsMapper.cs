using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to RoleDetails objects.
    /// </summary>
    public class RoleDetailsMapper : IRoleDetailsMapper
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public RoleDetailsMapper(
            IPermissionRepository permissionRepository,
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _permissionRepository = permissionRepository;
            _userAreaRepository = userAreaRepository;
        }

        /// <summary>
        /// Maps an EF Role record from the db into an RoleDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbRole">Role record from the database.</param>
        public virtual RoleDetails Map(Role dbRole)
        {
            if (dbRole == null) return null;

            var role = new RoleDetails()
            {
                IsAnonymousRole = dbRole.RoleCode == AnonymousRole.AnonymousRoleCode,
                IsSuperAdministrator = dbRole.RoleCode == SuperAdminRole.SuperAdminRoleCode,
                RoleId = dbRole.RoleId,
                RoleCode = dbRole.RoleCode,
                Title = dbRole.Title
            };

            var userArea = _userAreaRepository.GetByCode(dbRole.UserAreaCode);
            role.UserArea = new UserAreaMicroSummary()
            {
                UserAreaCode = dbRole.UserAreaCode,
                Name = userArea.Name
            };

            if (role.IsSuperAdministrator)
            {
                // Grant super users all permissions
                role.Permissions = _permissionRepository.GetAll().ToArray();
            }
            else
            {
                var permissions = new List<IPermission>(dbRole.RolePermissions.Count);

                foreach (var dbPermission in dbRole.RolePermissions.Select(rp => rp.Permission))
                {
                    var permission = _permissionRepository.GetByCode(dbPermission.PermissionCode, dbPermission.EntityDefinitionCode);
                    if (permission != null)
                    {
                        permissions.Add(permission);
                    }
                }

                role.Permissions = permissions.ToArray();
            }

            return role;
        }
    }
}
