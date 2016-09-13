using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PagePublishPermission : IEntityPermission
    {
        public PagePublishPermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = new PermissionType("PAGPUB", "Publish", "Publish or unpublish a page");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
