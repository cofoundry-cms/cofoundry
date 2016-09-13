using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RoleDeletePermission : IEntityPermission
    {
        public RoleDeletePermission()
        {
            EntityDefinition = new RoleEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Roles");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
