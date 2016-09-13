using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityDeletePermission : ICustomEntityPermissionTemplate
    {
        public CustomEntityDeletePermission()
        {
        }

        public CustomEntityDeletePermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = CommonPermissionTypes.Delete(customEntityDefinition.NamePlural);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityDeletePermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}
