using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RoleDetails
    {
        private readonly PermissionEqualityComparer _equalityComparer = new PermissionEqualityComparer();

        public int RoleId { get; set; }

        public string Title { get; set; }

        public bool IsSuperAdministrator { get; set; }

        public bool IsAnonymousRole { get; set; }

        public IPermission[] Permissions { get; set; }

        public UserAreaMicroSummary UserArea { get; set; }

        public bool HasPermission(IPermission permission)
        {
            return Permissions != null && Permissions.Contains(permission, _equalityComparer);
        }

        public bool HasPermission<TPermission>() where TPermission : IPermission, new()
        {
            var permission = new TPermission();
            return Permissions != null && Permissions.Contains(permission, _equalityComparer);
        }
    }
}
