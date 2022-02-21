using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserCommand : IPatchableCommand, ILoggableCommand
    {
        /// <summary>
        /// The first name is optional.
        /// </summary>
        [StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is optional.
        /// </summary>
        [StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// An optional display-friendly name. This is capped at 150 characters to
        /// match the <see cref="Username"/>, which may be used as the username in 
        /// some cases. If <see cref="UsernameOptions.UseAsDisplayName"/> is set to
        /// <see langword="true"/> then this field is ignored and the display name
        /// is instead copied from the normalized username.
        /// </summary>
        [StringLength(150)]
        public string DisplayName { get; set; }

        /// <summary>
        /// The email address is required if the user area has 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> set to <see langword="true"/>.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// The username is required if the user area has UseEmailAsUsername set to 
        /// false, otherwise it should be empty and the Email address will be used 
        /// as the username instead.
        /// </summary>
        public string Username { get; set; }
    }
}
