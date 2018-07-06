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
    public class TestRoleInitializer : IRoleInitializer<TestRole>
    {
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            return allPermissions
                .FilterToWritePermissions()
                .ExceptEntityPermissions<PageEntityDefinition>()
                .Union(allPermissions.FilterToReadPermissions())
                .Union(allPermissions
                    .FilterToAdminModulePermissions()
                    .ExceptEntityPermissions<PageEntityDefinition>())
                ;
        }
    }

    public class NewAnonymousRoleInitializer : IRoleInitializer<AnonymousRole>
    {
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            return allPermissions
                .FilterToUpdatePermissions()
                .Union(allPermissions.FilterToReadPermissions());
        }
    }
}
