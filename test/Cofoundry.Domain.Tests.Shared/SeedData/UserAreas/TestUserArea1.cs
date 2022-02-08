namespace Cofoundry.Domain.Tests.Shared
{
    /// <summary>
    /// Default area
    /// </summary>
    public class TestUserArea1 : IUserAreaDefinition
    {
        public const string Code = "TS1";

        public const string RecoveryUrlBase = "/ts1-auth/forgot-password";
        public const string VerificationUrlBase = "/ts1-auth/verify";
        public const string SignInPathSetting = "/area-1/sign-in";

        public string UserAreaCode => Code;

        public string Name => "Test Area 1";

        public bool AllowPasswordSignIn => true;

        public bool UseEmailAsUsername => true;

        public string SignInPath => SignInPathSetting;

        public bool IsDefaultAuthScheme => true;

        public void ConfigureOptions(UserAreaOptions options)
        {
            options.AccountRecovery.RecoveryUrlBase = RecoveryUrlBase;
            options.AccountVerification.VerificationUrlBase = VerificationUrlBase;
        }
    }
}
