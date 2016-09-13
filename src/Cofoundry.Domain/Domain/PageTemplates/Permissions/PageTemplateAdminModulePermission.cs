using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateAdminModulePermission : IEntityPermission
    {
        public PageTemplateAdminModulePermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
