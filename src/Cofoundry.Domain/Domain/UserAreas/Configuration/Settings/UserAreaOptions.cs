using Cofoundry.Core;
using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Configuration options for a user area, which are based on 
    /// <see cref="IdentitySettings"/> and can be customized via
    /// <see cref="IUserAreaDefinition.ConfigureOptions(UserAreaOptions)"/>.
    /// </summary>
    public class UserAreaOptions
    {
        /// <summary>
        /// Options used in the default password validation logic.
        /// </summary>
        public PasswordOptions Password { get; set; }

        public UsernameOptions Username { get; set; }

        public EmailAddressOptions EmailAddress { get; set; }

        /// <summary>
        /// Create a new <see cref="UserAreaOptions"/>, copying data from 
        /// the specified <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">
        /// <see cref="IdentitySettings"/> configuration to copy from.
        /// </param>
        public static UserAreaOptions CopyFrom(IdentitySettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.EmailAddress);
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.Password);
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.Username);

            var options = new UserAreaOptions()
            {
                EmailAddress = settings.EmailAddress.Clone(),
                Password = settings.Password.Clone(),
                Username = settings.Username.Clone()
            };

            return options;
        }
    }

}
