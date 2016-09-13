using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RoleUpdatePermission : IEntityPermission
    {
        public RoleUpdatePermission()
        {
            EntityDefinition = new RoleEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Roles");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
