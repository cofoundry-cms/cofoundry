using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea1RoleInitializer : IRoleInitializer<TestUserArea1Role>
    {
        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            return allPermissions;
        }
    }
}
