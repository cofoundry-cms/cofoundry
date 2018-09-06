using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Roles are an assignable collection of permissions. Every user has to be assigned 
    /// to one role.
    /// </summary>
    public partial class Role
    {
        public Role()
        {
            RolePermissions = new List<RolePermission>();
        }

        /// <summary>
        /// Database id of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The role title is used to identify the role and select it in the admin 
        /// UI and therefore must be unique. Max 50 characters.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The role code is a unique three letter code that can be used to reference the role 
        /// programatically. The code must be unique and convention is to use upper case, although 
        /// code matching is case insensitive. This is only used by roles defined in code using 
        /// IRoleDefinition.
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
        /// </summary>
        public virtual UserArea UserArea { get; set; }

        /// <summary>
        /// Collection of permissions that describe the actions this role is 
        /// permitted to perform.
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
