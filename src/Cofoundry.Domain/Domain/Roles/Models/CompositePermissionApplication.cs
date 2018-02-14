using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This applies multiple permissions in an 'OR' arrangement in that the
    /// permission evaluation is true if the user has at least one of the
    /// specified permissions.
    /// </summary>
    public class CompositePermissionApplication : IPermissionApplication
    {
        public CompositePermissionApplication()
        {
            Permissions = Array.Empty<IPermission>();
        }

        public CompositePermissionApplication(params IPermission[] permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        /// The collection of permissions to check. A user would need only one
        /// of these permissions to pass the evaluation of this application.
        /// </summary>
        public ICollection<IPermission> Permissions { get; set; }

        public override string ToString()
        {
            if (EnumerableHelper.IsNullOrEmpty(Permissions)) return base.ToString();

            return string.Join(", ", Permissions.Select(p => p.ToString()));
        }
    }
}
