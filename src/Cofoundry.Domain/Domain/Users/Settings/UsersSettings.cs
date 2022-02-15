using Cofoundry.Core.Configuration;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Global settings for users in all user areas. These settings can be overridden
    /// for an individual user area by implementing <see cref="IUserAreaDefinition.ConfigureOptions(UserAreaOptions)"/>.
    /// </summary>
    public class UsersSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Options used in the default password validation logic.
        /// </summary>
        [ValidateObject]
        public PasswordOptions Password { get; set; } = new PasswordOptions();

        /// <summary>
        /// Options to control the formatting and validation of usernames.
        /// Note that username character validation is ignored when 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
        /// <see langword="true"/>, because the format is already validated against
        /// the configured <see cref="EmailAddressOptions"/>.
        /// </summary>
        [ValidateObject]
        public UsernameOptions Username { get; set; } = new UsernameOptions();

        /// <summary>
        /// Options to control the formatting and validation of user email 
        /// addresses.
        /// </summary>
        [ValidateObject]
        public EmailAddressOptions EmailAddress { get; set; } = new EmailAddressOptions();

        /// <summary>
        /// Options to control the behavior of any authentication cookies.
        /// </summary>
        [ValidateObject]
        public CookieOptions Cookies { get; set; } = new CookieOptions();

        /// <summary>
        /// Options to control the behavior of the authentication process and related
        /// security features
        /// </summary>
        [ValidateObject]
        public AuthenticationOptions Authentication { get; set; } = new AuthenticationOptions();

        /// <summary>
        /// Options to control the behavior of the self-service account recovery feature.
        /// </summary>
        [ValidateObject]
        public AccountRecoveryOptions AccountRecovery { get; set; } = new AccountRecoveryOptions();

        /// <summary>
        /// Options to control the behavior of the account verification feature.
        /// Note that the Cofoundry admin panel does not support an account 
        /// verification flow and therefore these settings do not apply.
        /// </summary>
        [ValidateObject]
        public AccountVerificationOptions AccountVerification { get; set; } = new AccountVerificationOptions();

        /// <summary>
        /// Options to control the background task that runs to clean up 
        /// stale user data.
        /// </summary>
        [ValidateObject]
        public UserCleanupOptions Cleanup { get; set; } = new UserCleanupOptions();
    }
}