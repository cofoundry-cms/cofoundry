using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CofoundryUserDeletePermission : IEntityPermission
    {
        public CofoundryUserDeletePermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
