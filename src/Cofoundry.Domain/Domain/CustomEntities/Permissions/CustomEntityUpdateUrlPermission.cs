using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityUpdateUrlPermission : ICustomEntityPermissionTemplate
    {
        /// <summary>
        /// Constructor used internally by AuthorizePermissionAttribute.
        /// </summary>
        public CustomEntityUpdateUrlPermission()
        {
            PermissionType = CreatePermissionType("Not Set");
        }

        public CustomEntityUpdateUrlPermission(ICustomEntityDefinition customEntityDefinition)
        {
            EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
            PermissionType = new PermissionType("UPDURL", "Update custom entity Url", "Update the url of a " + customEntityDefinition.Name);
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }

        public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
        {
            var implementedPermission = new CustomEntityUpdateUrlPermission(customEntityDefinition);
            return implementedPermission;
        }

        private static PermissionType CreatePermissionType(string customEntityName)
        {
            return new PermissionType("UPDURL", "Update custom entity Url", "Update the url of a " + customEntityName);
        }
    }
}
