using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class GeneralSettingsUpdatePermission : IEntityPermission
    {
        public GeneralSettingsUpdatePermission()
        {
            EntityDefinition = new SettingsEntityDefinition();
            PermissionType = new PermissionType("GENUPD", "Update General", "Update General Settings");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
