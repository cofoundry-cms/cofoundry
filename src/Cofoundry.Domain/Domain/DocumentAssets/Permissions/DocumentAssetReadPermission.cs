using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DocumentAssetReadPermission : IEntityPermission
    {
        public DocumentAssetReadPermission()
        {
            EntityDefinition = new DocumentAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Document Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
