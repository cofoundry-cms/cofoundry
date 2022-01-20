namespace Cofoundry.Domain.Tests.Shared
{
    /// <summary>
    /// Default area
    /// </summary>
    public class TestUserArea1 : IUserAreaDefinition
    {
        public const string Code = "TS1";

        public const string RecoveryUrlPath = "/ts1-auth/forgot-password";

        public string UserAreaCode => Code;

        public string Name => "Test Area 1";

        public bool AllowPasswordLogin => true;

        public bool UseEmailAsUsername => true;

        public string LoginPath => "/area-1/login";

        public bool IsDefaultAuthScheme => true;

        public void ConfigureOptions(UserAreaOptions options)
        {
            options.AccountRecovery.RecoveryUrlBase = RecoveryUrlPath;
        }
    }
}
