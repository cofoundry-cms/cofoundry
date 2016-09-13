using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateUpdatePermission : IEntityPermission
    {
        public PageTemplateUpdatePermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
