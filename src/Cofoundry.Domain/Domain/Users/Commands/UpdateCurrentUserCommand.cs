using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserCommand : ICommand, ILoggableCommand
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
