using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class WebDirectoryCreatePermission : IEntityPermission
    {
        public WebDirectoryCreatePermission()
        {
            EntityDefinition = new WebDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Web Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
