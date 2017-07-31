using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class TestRole : IRoleDefinition
    {
        public const string TestRoleCode = "TST";

        public string Title { get { return "Test"; } }

        public string RoleCode { get { return TestRoleCode; } }

        public string UserAreaCode { get { return CofoundryAdminUserArea.AreaCode; } }
    }
}
