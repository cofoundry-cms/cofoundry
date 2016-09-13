using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageUpdatePermission : IEntityPermission
    {
        public PageUpdatePermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Pages");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
