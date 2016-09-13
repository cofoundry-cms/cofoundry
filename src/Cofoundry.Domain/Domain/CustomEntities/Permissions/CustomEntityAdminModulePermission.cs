using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityAdminModulePermission : ICustomEntityPermissionTemplate
    {
        public CustomEntityAdminModulePermission()
        {
        }

        public CustomEntityAdminModulePermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.AdminModule(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityAdminModulePermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}
