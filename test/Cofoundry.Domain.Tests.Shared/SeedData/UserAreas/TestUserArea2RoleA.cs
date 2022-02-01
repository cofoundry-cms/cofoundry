using System.Collections.Generic;

namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2RoleA : IRoleDefinition, IRoleInitializer<TestUserArea2RoleA>
    {
        public const string Code = "T2A";

        public string Title => "Test User Area 2 Role A";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea2.Code;

        public IEnumerable<IPermission> GetPermissions(IEnumerable<IPermission> allPermissions)
        {
            return allPermissions;
        }
    }
}
