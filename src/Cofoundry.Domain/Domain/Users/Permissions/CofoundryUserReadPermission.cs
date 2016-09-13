using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <remarks>
    /// Note that user permissions are divided between Cofoundry Admin users and all other
    /// user areas. I don't think this needs to be more granular than this so
    /// for the sake of simplicity they are grouped in this way.
    /// </remarks>
    public class CofoundryUserReadPermission : IEntityPermission
    {
        public CofoundryUserReadPermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
