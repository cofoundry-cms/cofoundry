using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Integration
{
    public class TestRole1 : IRoleDefinition
    {
        public const string Code = "TS1";

        public string Title => "Test Role 1";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea1.Code;
    }
}
