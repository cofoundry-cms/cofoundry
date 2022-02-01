namespace Cofoundry.Domain.Tests.Shared.SeedData
{
    /// <summary>
    /// AllowPasswordLogin = <see langword="false"/>,
    /// UseEmailAsUsername = <see langword="true"/>
    /// </summary>
    public class UserAreaWithoutPasswordLogin : IUserAreaDefinition
    {
        public const string Code = "NPW";

        public string UserAreaCode => Code;

        public string Name => "No PW Login";

        public bool AllowPasswordLogin => false;

        public bool UseEmailAsUsername => false;

        public string LoginPath => "/npw/login";

        public bool IsDefaultAuthScheme => false;
    }
}
