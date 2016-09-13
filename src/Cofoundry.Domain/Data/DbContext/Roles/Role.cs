using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Role
    {
        public Role()
        {
            Permissions = new List<Permission>();
        }

        public int RoleId { get; set; }
        public string Title { get; set; }
        public string SpecialistRoleTypeCode { get; set; }

        public string UserAreaCode { get; set; }
        public virtual UserArea UserArea { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
