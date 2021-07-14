using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Integration
{
    public class TestRole2 : IRoleDefinition
    {
        public const string Code = "TS2";

        public string Title => "Test Role 2";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea2.Code;
    }
}
