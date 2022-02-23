namespace Cofoundry.Domain.Tests.Shared.SeedData
{
    /// <summary>
    /// AllowPasswordLogin = <see langword="false"/>,
    /// UseEmailAsUsername = <see langword="true"/>
    /// </summary>
    public class UserAreaWithoutPasswordSignIn : IUserAreaDefinition
    {
        public const string Code = "NPW";

        public string UserAreaCode => Code;

        public string Name => "No PW Login";

        public bool AllowPasswordSignIn => false;

        public bool UseEmailAsUsername => false;

        public string SignInPath => "/npw/login";

        public bool IsDefaultAuthScheme => false;

        public void ConfigureOptions(UserAreaOptions options)
        {
        }
    }
}
