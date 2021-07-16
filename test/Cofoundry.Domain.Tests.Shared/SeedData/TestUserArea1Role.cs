using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Shared
{
    /// <summary>
    /// This test role has all permissions.
    /// </summary>
    public class TestUserArea1Role : IRoleDefinition
    {
        public const string Code = "TS1";

        public string Title => "Test User Area 1 Role";

        public string RoleCode => Code;

        public string UserAreaCode => TestUserArea1.Code;
    }
}
