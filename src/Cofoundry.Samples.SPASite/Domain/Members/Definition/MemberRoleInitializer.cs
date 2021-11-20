using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// A role initializer allows you to define the permissions that
    /// should be added to a role in defined code. This is typically only used
    /// when a role is first created, but could be used to update permissions
    /// if running RegisterDefinedRolesCommand manually.
    /// </summary>
    public class MemberRoleInitializer : IRoleInitializer<MemberRole>
    {
        /// <summary>
        /// This method determins what permissions the role has. To help do this
        /// you are provided with a collection of all permissions in the system and
        /// you can query the collection to either filter out permissions you dont want
        /// or create a new collection and mix permissions in that you do want. There are
        /// a number of extension method on the collection that make this easier.
        /// </summary>
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            // In this example application we don't require any additional permissions for members
            // so we can re-use the permission set on the anonymous role which include read access 
            // to most entities.
            return allPermissions
                .FilterToAnonymousRoleDefaults()
                ;
        }
    }
}
