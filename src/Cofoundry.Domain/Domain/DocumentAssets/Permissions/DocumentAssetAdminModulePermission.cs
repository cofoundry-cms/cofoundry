using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DocumentAssetAdminModulePermission : IEntityPermission
    {
        public DocumentAssetAdminModulePermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
