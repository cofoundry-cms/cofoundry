using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageAdminModulePermission : IEntityPermission
    {
        public PageAdminModulePermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Pages");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
