using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityUpdatePermission : ICustomEntityPermissionTemplate
    {
        public CustomEntityUpdatePermission()
        {
        }

        public CustomEntityUpdatePermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.Update(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityUpdatePermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}
