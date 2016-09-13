using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateDeletePermission : IEntityPermission
    {
        public PageTemplateDeletePermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
