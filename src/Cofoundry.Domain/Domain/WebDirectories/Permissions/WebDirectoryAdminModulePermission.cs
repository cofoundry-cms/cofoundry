using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class WebDirectoryAdminModulePermission : IEntityPermission
    {
        public WebDirectoryAdminModulePermission()
        {
            EntityDefinition = new WebDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Web Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
