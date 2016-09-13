using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ImageAssetAdminModulePermission : IEntityPermission
    {
        public ImageAssetAdminModulePermission()
        {
            EntityDefinition = new ImageAssetEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Image Assets");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
