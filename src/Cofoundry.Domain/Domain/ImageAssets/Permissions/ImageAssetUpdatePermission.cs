using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ImageAssetUpdatePermission : IEntityPermission
    {
        public ImageAssetUpdatePermission()
        {
            EntityDefinition = new ImageAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Image Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
