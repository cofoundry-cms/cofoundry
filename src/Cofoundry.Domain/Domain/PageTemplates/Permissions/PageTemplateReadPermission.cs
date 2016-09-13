using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateReadPermission : IEntityPermission
    {
        public PageTemplateReadPermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
