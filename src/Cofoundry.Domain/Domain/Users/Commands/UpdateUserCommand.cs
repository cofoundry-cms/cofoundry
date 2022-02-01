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

        /// <summary>
        /// The email address is required if the user area has 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> set to <see langword="true"/>.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The username is required if the user area has UseEmailAsUsername set to 
        /// false, otherwise it should be empty and the Email address will be used 
        /// as the username instead.
        /// </summary>
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
        /// Indicates whether the user account is marked as verified or activated. One 
        /// common way of verification is via an email sign-up notification. Given that account 
        /// verification is not tied to any specified data, it is not automatically
        /// cleared when for example an email address is updated. The management of this property
        /// is left to the implementor.
        /// </summary>
        public bool IsAccountVerified { get; set; }

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
