namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2 : IUserAreaDefinition
    {
        public const string Code = "TS2";

        public string UserAreaCode => Code;

        public string Name => "Test Area 2";

        public bool AllowPasswordLogin => true;

        public bool UseEmailAsUsername => true;

        public string LoginPath => "/area-2/login";

        public bool IsDefaultAuthScheme => false;
    }
}
