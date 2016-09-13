using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityPublishPermission : ICustomEntityPermissionTemplate
    {
        public CustomEntityPublishPermission()
        {
        }

        public CustomEntityPublishPermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = new PermissionType("CMEPUB", "Publish", "Publish or unpublish a " + customEntityDefinition.Name.ToLower());
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityPublishPermission(customEntityDefinition);
            return implementedPermission;
        }
    }
}
