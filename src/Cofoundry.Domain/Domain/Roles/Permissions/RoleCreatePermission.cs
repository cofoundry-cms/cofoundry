using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RoleCreatePermission : IEntityPermission
    {
        public RoleCreatePermission()
        {
            EntityDefinition = new RoleEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Roles");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
