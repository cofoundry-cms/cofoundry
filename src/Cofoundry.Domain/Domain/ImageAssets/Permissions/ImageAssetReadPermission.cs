using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ImageAssetReadPermission : IEntityPermission
    {
        public ImageAssetReadPermission()
        {
            EntityDefinition = new ImageAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Image Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
