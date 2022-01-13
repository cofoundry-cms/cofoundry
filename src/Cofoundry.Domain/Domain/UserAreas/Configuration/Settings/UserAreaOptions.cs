using Cofoundry.Core;
using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Configuration options for a user area, which are based on 
    /// <see cref="UsersSettings"/> and can be customized via
    /// <see cref="IUserAreaDefinition.ConfigureOptions(UserAreaOptions)"/>.
    /// </summary>
    public class UserAreaOptions
    {
        /// <summary>
        /// Options used in the default password validation logic.
        /// </summary>
        public PasswordOptions Password { get; set; }

        /// <summary>
        /// Options to control the formatting and validation of usernames.
        /// Note that username character validation is ignored when 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
        /// <see langword="true"/>, because the format is already validated against
        /// the configured <see cref="EmailAddressOptions"/>.
        /// </summary>
        public UsernameOptions Username { get; set; }

        /// <summary>
        /// Options to control the formatting and validation of user email 
        /// addresses.
        /// </summary>
        public EmailAddressOptions EmailAddress { get; set; }

        /// <summary>
        /// Options to control the behavior of any authentication cookies.
        /// </summary>
        public CookieOptions Cookies { get; set; } = new CookieOptions();

        /// <summary>
        /// Create a new <see cref="UserAreaOptions"/>, copying data from 
        /// the specified <paramref name="settings"/>.
        /// </summary>
        /// <param name="settings">
        /// <see cref="UsersSettings"/> configuration to copy from.
        /// </param>
        public static UserAreaOptions CopyFrom(UsersSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.EmailAddress);
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.Password);
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.Username);
            EntityInvalidOperationException.ThrowIfNull(settings, s => s.Cookies);

            var options = new UserAreaOptions()
            {
                EmailAddress = settings.EmailAddress.Clone(),
                Password = settings.Password.Clone(),
                Username = settings.Username.Clone(),
                Cookies = settings.Cookies.Clone()
            };

            return options;
        }
    }
}