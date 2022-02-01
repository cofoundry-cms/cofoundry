namespace Cofoundry.Domain.Tests.Shared.SeedData
{
    /// <summary>
    /// AllowPasswordLogin = <see langword="true"/>,
    /// UseEmailAsUsername = <see langword="false"/>
    /// </summary>
    public class UserAreaWithoutEmailAsUsername : IUserAreaDefinition
    {
        public const string Code = "NEU";

        public string UserAreaCode => Code;

        public string Name => "Username not email";

        public bool AllowPasswordLogin => true;

        public bool UseEmailAsUsername => false;

        public string LoginPath => "/weu/login";

        public bool IsDefaultAuthScheme => false;
    }
}
