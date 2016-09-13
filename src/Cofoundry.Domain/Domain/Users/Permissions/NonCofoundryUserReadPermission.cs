using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class NonCofoundryUserReadPermission : IEntityPermission
    {
        public NonCofoundryUserReadPermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
