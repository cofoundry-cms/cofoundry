using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DocumentAssetUpdatePermission : IEntityPermission
    {
        public DocumentAssetUpdatePermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
