using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageDirectoryReadPermission : IEntityPermission
    {
        public PageDirectoryReadPermission()
        {
            EntityDefinition = new PageDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Page Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
