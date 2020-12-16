using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A fully featured role projection including all properties and 
    /// permission information.
    /// </summary>
    public class RoleDetails
    {
        private readonly PermissionEqualityComparer _equalityComparer = new PermissionEqualityComparer();

        /// <summary>
        /// Database id of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The title is used to identify the role and select it in the admin UI
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// True if the role is the system level "Super Admin" role.
        /// </summary>
        public bool IsSuperAdministrator { get; set; }

        /// <summary>
        /// True if the role is the special anonymous role assigned to unauthenticated users.
        /// </summary>
        public bool IsAnonymousRole { get; set; }

        /// <summary>
        /// The specialist role type code is a unique three letter code that
        /// can be used to reference the role programatically. The code must be unique
        /// and convention is to use upper case, although code matching is case insensitive.
        /// This is only used by roles defined in code using IRoleDefinition.
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// Collection of permissions that describe the actions this role is 
        /// permitted to perform.
        /// </summary>
        public ICollection<IPermission> Permissions { get; set; }

        /// <summary>
        /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
        /// </summary>
        public UserAreaMicroSummary UserArea { get; set; }

        /// <summary>
        /// Determins if the role has the specified permission
        /// </summary>
        /// <param name="permission">Permission to check</param>
        /// <returns>True if the role has the permission; otherwise false</returns>
        public bool HasPermission(IPermission permission)
        {
            return Permissions != null && Permissions.Contains(permission, _equalityComparer);
        }

        /// <summary>
        /// Determins if the role has the specified permission
        /// </summary>
        /// <typeparam name="TPermission">Type of permission to check</typeparam>
        /// <returns>True if the role has the permission; otherwise false</returns>
        public bool HasPermission<TPermission>() where TPermission : IPermission, new()
        {
            var permission = new TPermission();
            return Permissions != null && Permissions.Contains(permission, _equalityComparer);
        }
    }
}
