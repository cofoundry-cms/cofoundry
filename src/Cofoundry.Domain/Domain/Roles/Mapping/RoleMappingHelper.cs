using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class RoleMappingHelper
    {
        private readonly IPermissionRepository _permissionRepository;

        public RoleMappingHelper(
            IPermissionRepository permissionRepository
            )
        {
            _permissionRepository = permissionRepository;
        }

        public RoleDetails MapDetails(Role dbRole)
        {
            if (dbRole == null) return null;

            var role = Mapper.Map<RoleDetails>(dbRole);

            if (role.IsSuperAdministrator)
            {
                // Grant super users all permissions
                role.Permissions = _permissionRepository.GetAll().ToArray();
            }
            else
            {
                var permissions = new List<IPermission>(dbRole.Permissions.Count);

                foreach (var dbPermission in dbRole.Permissions)
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
