using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class WebDirectoryDeletePermission : IEntityPermission
    {
        public WebDirectoryDeletePermission()
        {
            EntityDefinition = new WebDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Web Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
