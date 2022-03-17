namespace Cofoundry.Domain;

/// <summary>
/// Validates a user email address, returning any errors found. By default the validator
/// checks that the format contains only the characters permitted by the 
/// <see cref="EmailAddressOptions"/> configuration settings, as well as checking
/// for uniqueness if necessary.
/// </summary>
public class ValidateUserEmailAddressQuery : IQuery<ValidationQueryResult>
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to check.
    /// This is required because formatting and uniqueness configuration can be
    /// different for each user area.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The email address to validate. The email will be normalized before the check is 
    /// made so there is no need to pre-format this value.
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Database id of an existing user. If the user is new then this can be <see langword="null"/>.
    /// </summary>
    public int? UserId { get; set; }
}
