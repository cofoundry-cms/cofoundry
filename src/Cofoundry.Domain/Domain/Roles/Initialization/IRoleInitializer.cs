using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A role initializer allows you to define the permissions that
    /// should be added to a role defined in code. This is typically only used
    /// when a role is first created, but could be used to update permissions
    /// if running RegisterDefinedRolesCommand manually.
    /// </summary>
    /// <typeparam name="TRoleDefinition">The role type to define permissions for</typeparam>
    public interface IRoleInitializer<TRoleDefinition> : IRoleInitializer
        where TRoleDefinition : IRoleDefinition
    {
    }

    /// <summary>
    /// Non-generic version of IRoleInitializer&lt;&gt; not to be implemented directly.
    /// </summary>
    public interface IRoleInitializer
    {
        /// <summary>
        /// Filters a collection of all permissions in the system to include
        /// only the permissions that should be added to the role. The extensions
        /// in IPermissionExtensions can help make this easier to do.
        /// </summary>
        /// <param name="allPermissions">A collection containing every permission in the system.</param>
        /// <returns>A set of permissions filtered to only include the permissions that should be added to the role</returns>
        IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions);
    }
}
