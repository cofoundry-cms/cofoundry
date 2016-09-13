using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Permission
    {
        public int PermissionId { get; set; }
        public string EntityDefinitionCode { get; set; }
        public string PermissionCode { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
