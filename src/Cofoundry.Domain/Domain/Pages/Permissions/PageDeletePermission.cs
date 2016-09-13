using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageDeletePermission : IEntityPermission
    {
        public PageDeletePermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Pages");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
