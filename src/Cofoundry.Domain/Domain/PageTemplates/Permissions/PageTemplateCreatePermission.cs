using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateCreatePermission : IEntityPermission
    {
        public PageTemplateCreatePermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
