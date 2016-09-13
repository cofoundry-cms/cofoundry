using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class SettingsAdminModulePermission : IEntityPermission
    {
        public SettingsAdminModulePermission()
        {
            EntityDefinition = new SettingsEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Settings");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
