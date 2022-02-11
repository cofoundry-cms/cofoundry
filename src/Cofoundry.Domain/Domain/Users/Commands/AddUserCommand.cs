using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic user creation command that adds data only and does not 
    /// send any email notifications.
    /// </summary>
    /// <remarks>
    /// Sealed because we should be setting these properties
    /// explicitly and shouldn't allow any possible injection of passwords or
    /// user areas.
    /// </remarks>
    public sealed class AddUserCommand : ICommand, ILoggableCommand, IValidatableObject
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
        /// The password is required if the user area has <see cref="IUserAreaDefinition.AllowPasswordSignIn"/> 
        /// set to <see langword="true"/>, otherwise it should be empty. The password will go through
        /// additional validation depending on the password policy configuration.
        /// </summary>
        [StringLength(PasswordOptions.MAX_LENGTH_BOUNDARY, MinimumLength = PasswordOptions.MIN_LENGTH_BOUNDARY)]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// The email address is required if the user area has <see cref="IUserAreaDefinition.UseEmailAsUsername"/> 
        /// set to <see langword="true"/> or <see cref="IUserAreaDefinition.AllowPasswordSignIn"/>.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// The username is required if the user area has <see cref="IUserAreaDefinition.UseEmailAsUsername"/> 
        /// set to <see langword="false"/>, otherwise it should be empty and the <see cref="Email"/> will be used 
        /// as the username instead.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Indicates whether the user will be prompted to change their password the
        /// next time they log in. Defaults to <see langword="false"/>.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// Indicates whether the user should be marked as verified. Account verification is
        /// an optional process that usually happens as part of a two-step verification flow
        /// via an out-of-band verificaton notification. Defaults to <see langword="false"/>.
        /// </summary>
        public bool IsAccountVerified { get; set; }

        /// <summary>
        /// Setting <see cref="IsActive"/> to <see langword="false"/> deactivates
        /// a user, preventing them from logging in or taking any actions, but does
        /// not remove any data or prevent them from being queried. Defaults
        /// to <see langword="true"/>.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// The Cofoundry user system is partitioned into user areas a user
        /// must belong to one of these user areas.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }

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
        /// The database id of the newly created user. This is set after the 
        /// command has been run.
        /// </summary>
        [OutputValue]
        public int OutputUserId { get; set; }

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
