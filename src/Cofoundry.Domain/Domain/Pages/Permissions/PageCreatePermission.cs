using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageCreatePermission : IEntityPermission
    {
        public PageCreatePermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Pages");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
