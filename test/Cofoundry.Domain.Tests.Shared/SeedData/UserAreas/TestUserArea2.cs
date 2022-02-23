namespace Cofoundry.Domain.Tests.Shared
{
    public class TestUserArea2 : IUserAreaDefinition
    {
        public const string Code = "TS2";

        public string UserAreaCode => Code;

        public string Name => "Test Area 2";

        public bool AllowPasswordSignIn => true;

        public bool UseEmailAsUsername => true;

        public string SignInPath => "/area-2/sign-in";

        public bool IsDefaultAuthScheme => false;

        public void ConfigureOptions(UserAreaOptions options)
        {
        }
    }
}
