using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageReadPermission : IEntityPermission
    {
        public IEntityDefinition EntityDefinition => new PageEntityDefinition();

        public PermissionType PermissionType => CommonPermissionTypes.Read("Pages");
    }
}
