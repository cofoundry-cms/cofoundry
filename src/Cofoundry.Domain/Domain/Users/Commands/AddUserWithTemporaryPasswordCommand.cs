using Newtonsoft.Json;

namespace Cofoundry.Domain;

/// <summary>
/// Adds a new user and sends a notification containing a generated 
/// password which must be changed at first sign in.
/// </summary>
/// <remarks>
/// Sealed because we should be setting these properties
/// explicitly and shouldn't allow any possible injection of passwords or
/// user areas.
/// </remarks>
public sealed class AddUserWithTemporaryPasswordCommand : ICommand, ILoggableCommand, IValidatableObject
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
    /// An optional display-friendly name. This is capped at 150 characters to
    /// match the <see cref="Username"/>, which may be used as the username in 
    /// some cases. If <see cref="UsernameOptions.UseAsDisplayName"/> is set to
    /// <see langword="true"/> then this field is ignored and the display name
    /// is instead copied from the normalized username.
    /// </summary>
    [StringLength(150)]
    public string DisplayName { get; set; }

    /// <summary>
    /// An email address is required for this command so that an email
    /// notification can be sent.
    /// </summary>
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    /// <summary>
    /// The username is required if the user area has <see cref="IUserAreaDefinition.UseEmailAsUsername"/> 
    /// set to <see langword="false"/>, otherwise it should be empty and the <see cref="Email"/> will be used 
    /// as the username instead.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The Cofoundry user system is partitioned into user areas, a user
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
    /// The database id of the newly created user. This is set after the command
    /// has been run.
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
