using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the password of an unathenticated user, using the
    /// credentials in the command to authenticate the request.
    /// </summary>
    public class UpdateUnauthenticatedUserPasswordCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The unique code of the user area match logins for. Note
        /// that usernames are unique per user area and therefore a given username
        /// may have an account for more than one user area.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }

        #region Output

        /// <summary>
        /// The database id of the updated user. This is set after the command
        /// has been run.
        /// </summary>
        [OutputValue]
        public int OutputUserId { get; set; }

        #endregion
    }
}
