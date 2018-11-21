using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds a new user and sends a notification containing a generated 
    /// password which must be changed at first login.
    /// </summary>
    /// <remarks>
    /// Sealed because we should be setting these properties
    /// explicitly and shouldn't allow any possible injection of passwords or
    /// user areas.
    /// </remarks>
    public sealed class AddUserWithTemporaryPasswordCommand : ICommand, ILoggableCommand
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
        /// An email address is required for this command so that an email
        /// notification can be sent.
        /// </summary>
        [Required]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// The Cofoundry user system is partitioned into user areas a user
        /// must belong to one of these user areas.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The role that this user is assigned to. The role is required and
        /// determines the permissions available to the user. This must be a role
        /// that belongs to the Cofoundry admin user area.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }

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
