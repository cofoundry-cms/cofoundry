using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A generic user update command for use with Cofoundry users and
    /// other non-Cofoundry users.
    /// </summary>
    public class UpdateUserCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// Database id of the user to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

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

        private string _email = null;
        /// <summary>
        /// The email address is required if the user area has 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> set to <see langword="true"/>.
        /// </summary>
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string Email
        {
            get { return _email; }
            set { _email = value == string.Empty ? null : value; }
        }

        /// <summary>
        /// The username is required if the user area has UseEmailAsUsername set to 
        /// false, otherwise it should be empty and the Email address will be used 
        /// as the username instead.
        /// </summary>
        [StringLength(150)]
        public string Username { get; set; }

        /// <summary>
        /// The id of the role that this user is assigned to. Either the
        /// RoleId or RoleCode property must be filled in, but not both. The 
        /// role is required and determines the permissions available to the user. 
        /// </summary>
        [PositiveInteger]
        public int? RoleId { get; set; }

        /// <summary>
        /// The code for the role that this user is assigned to. Either the
        /// RoleId or RoleCode property must be filled in, but not both. The 
        /// role is required and determines the permissions available to the user.
        /// </summary>
        [StringLength(3)]
        public string RoleCode { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(RoleCode) && !RoleId.HasValue)
            {
                yield return new ValidationResult("Either a role id or role code must be defined.", new string[] { nameof(RoleId) });
            }

            if (!string.IsNullOrWhiteSpace(RoleCode) && RoleId.HasValue)
            {
                yield return new ValidationResult("Either a role id or role code must be defined, not both.", new string[] { nameof(RoleId) });
            }
        }
    }
}
