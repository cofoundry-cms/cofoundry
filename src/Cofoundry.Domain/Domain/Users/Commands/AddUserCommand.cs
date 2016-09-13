using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A generic user creation command for use with Cofoundry users and
    /// other non-Cofoundry users. Does not send any email notifications.
    /// </summary>
    /// <remarks>
    /// Sealed because we should be setting these properties
    /// explicitly and shouldn't allow any possible injection of passwords or
    /// user areas.
    /// </remarks>
    public sealed class AddUserCommand : ICommand, ILoggableCommand
    {
        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// The password is required if the user area has AllowPasswordLogin set to 
        /// true, otherwise it should be empty.
        /// </summary>
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [XmlIgnore]
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// When set to true, this will egenrate a random password for the user.
        /// </summary>
        public bool GeneratePassword { get; set; }

        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// The username is required if the user area has UseEmailAsUsername set to 
        /// false, otherwise it should be empty and the Email address will be used 
        /// as the username instead.
        /// </summary>
        [StringLength(150)]
        public string Username { get; set; }

        public bool RequirePasswordChange { get; set; }

        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }

        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }

        #region Output

        [OutputValue]
        public int OutputUserId { get; set; }

        #endregion
    }
}
