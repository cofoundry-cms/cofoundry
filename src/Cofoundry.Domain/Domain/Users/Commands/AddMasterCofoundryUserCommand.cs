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
    /// Creates the initial super admin account for the site as part
    /// of the setup process. This cannot be run once the site is set up
    /// because by design it has to forgo permission checks.
    /// </summary>
    /// <remarks>
    /// Sealed because we should be setting these properties
    /// explicitly and shouldn't allow any possible injection of passwords or
    /// user areas.
    /// </remarks>
    public sealed class AddMasterCofoundryUserCommand : ICommand, ILoggableCommand
    {
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
        /// The emaill address is required and is used as the login username
        /// for the user.
        /// </summary>
        [Required]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// True if a password change should be required when first logging on. The
        /// default value is false for the master user.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        #region Output

        /// <summary>
        /// The database id of the newly created user. This is set after the command
        /// has been run.
        /// </summary>
        [OutputValue]
        public int OutputUserId { get; set; }

        #endregion
    }
}
