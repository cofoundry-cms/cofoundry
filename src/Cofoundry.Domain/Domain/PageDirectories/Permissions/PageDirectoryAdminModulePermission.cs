using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageDirectoryAdminModulePermission : IEntityPermission
    {
        public PageDirectoryAdminModulePermission()
        {
            EntityDefinition = new PageDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Page Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
