using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the password of an unathenticated user, using the
    /// credentials in the command to authenticate the request.
    /// </summary>
    public class UpdateUserPasswordByCredentialsCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The unique code of the user area match logins for. Note
        /// that usernames are unique per user area and therefore a given username
        /// may have an account for more than one user area.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The users username, which is required to authenticate
        /// the user before performing the update.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The users existing password. This is required to authenticate
        /// the user before performing the update.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string OldPassword { get; set; }

        /// <summary>
        /// The value to set as the new account password. The password will go through additional validation depending 
        /// on the password policy configuration.
        /// </summary>
        [Required]
        [StringLength(PasswordOptions.MAX_LENGTH_BOUNDARY, MinimumLength = PasswordOptions.MIN_LENGTH_BOUNDARY)]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }

        /// <summary>
        /// The database id of the updated user. This is set after the command
        /// has been run.
        /// </summary>
        [OutputValue]
        public int OutputUserId { get; set; }
    }
}
