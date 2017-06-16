using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Data
{
    public class RolePermission
    {
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
