using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2Role : IRoleDefinition
    {
        public const string Code = "TS2";

        public string Title => "Test User Area 2 Role";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea2.Code;
    }
}
