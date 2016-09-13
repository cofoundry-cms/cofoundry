using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityReadPermission : ICustomEntityPermissionTemplate
    {
        public CustomEntityReadPermission()
        {
        }

        public CustomEntityReadPermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.Read(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityReadPermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}
