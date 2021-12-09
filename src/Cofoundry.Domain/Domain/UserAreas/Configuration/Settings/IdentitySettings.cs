using Cofoundry.Core.Configuration;

namespace Cofoundry.Domain
{
    public class IdentitySettings : CofoundryConfigurationSettingsBase
    {
        public PasswordOptions Password { get; set; } = new PasswordOptions();

        public UsernameOptions Username { get; set; } = new UsernameOptions();

        public EmailAddressOptions EmailAddress { get; set; } = new EmailAddressOptions();
    }

}
