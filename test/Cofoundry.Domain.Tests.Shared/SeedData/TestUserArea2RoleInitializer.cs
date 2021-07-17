using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2RoleInitializer : IRoleInitializer<TestUserArea2Role>
    {
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            return allPermissions;
        }
    }
}
