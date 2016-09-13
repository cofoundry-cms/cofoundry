using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageUpdateUrlPermission : IEntityPermission
    {
        public PageUpdateUrlPermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = new PermissionType("UPDURL", "Update Page Url", "Update the url of a page");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
