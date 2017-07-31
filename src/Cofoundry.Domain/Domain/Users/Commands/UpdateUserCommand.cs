using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A generic user update command for use with Cofoundry users and
    /// other non-Cofoundry users.
    /// </summary>
    public class UpdateUserCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the user to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

        /// <summary>
        /// The first name is required.
        /// </summary>
        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is required.
        /// </summary>
        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// The email address is required if the user area has UseEmailAsUsername 
        /// set to true.
        /// </summary>
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string Email { get; set; }

        /// <summary>
        /// The username is required if the user area has UseEmailAsUsername set to 
        /// false, otherwise it should be empty and the Email address will be used 
        /// as the username instead.
        /// </summary>
        [StringLength(150)]
        public string Username { get; set; }

        /// <summary>
        /// The role that this user is assigned to. The role is required and
        /// determines the permissions available to the user.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }

        /// <summary>
        /// Indicates whether the user will be prompted to change their password the
        /// next time they log in.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// A flag to indicate if the users email address has been confirmed via a 
        /// sign-up notification.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
    }
}
