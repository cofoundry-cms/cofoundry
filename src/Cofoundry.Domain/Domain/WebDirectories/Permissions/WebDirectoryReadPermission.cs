using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class WebDirectoryReadPermission : IEntityPermission
    {
        public WebDirectoryReadPermission()
        {
            EntityDefinition = new WebDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Web Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
