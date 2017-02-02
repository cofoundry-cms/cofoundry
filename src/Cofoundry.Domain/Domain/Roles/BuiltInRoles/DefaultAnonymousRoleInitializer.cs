using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// We use a default initializer just for the anonymous role
    /// as it's the only built in role with permissions. This allows
    /// for you to implement a custom role initizlier for the anonymous
    /// role in your application to override the default one.
    /// </summary>
    public class DefaultAnonymousRoleInitializer : IRoleInitializer
    {
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            // The anonymous role by default can read any entity except for users
            // This is because user read permission means 'all users' not just 'current user'
            return allPermissions
                .FilterToAnonymousRoleDefaults();
        }
    }
}
