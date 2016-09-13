using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CurrentUserUpdatePermission : IEntityPermission
    {
        public CurrentUserUpdatePermission()
        {
            EntityDefinition = new CurrentUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Current User");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
