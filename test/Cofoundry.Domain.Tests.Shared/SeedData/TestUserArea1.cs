using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Tests.Shared
{
    /// <summary>
    /// Default area
    /// </summary>
    public class TestUserArea1 : IUserAreaDefinition
    {
        public const string Code = "TS1";

        public string UserAreaCode => Code;

        public string Name => "Test Area 1";

        public bool AllowPasswordLogin => true;

        public bool UseEmailAsUsername => true;

        public string LoginPath => "/login";

        public bool IsDefaultAuthSchema => true;
    }
}
