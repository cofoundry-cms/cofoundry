using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CofoundryUserUpdatePermission : IEntityPermission
    {
        public CofoundryUserUpdatePermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
