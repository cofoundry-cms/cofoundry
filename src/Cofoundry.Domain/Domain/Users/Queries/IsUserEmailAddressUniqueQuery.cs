namespace Cofoundry.Domain;

/// <summary>
/// Determines if an email address is unique within a user area. Email
/// addresses must be unique per user area and can therefore appear in multiple
/// user areas.
/// </summary>
public class IsUserEmailAddressUniqueQuery : IQuery<bool>
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to check.
    /// This is required because email addresses only have to be unique per user area.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// The email address to check for uniqueness. The email will
    /// be "uniquified" before the check is made so there is no need
    /// to pre-format this value. Null or empty values are not valid 
    /// but will return <see langword="true"/> because although uniqueness 
    /// validation should not be triggered for these values it is technically 
    /// the correct answer.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Optional database id of an existing user to exclude from the uniqueness 
    /// check. Use this when checking the uniqueness of an existing users email.
    /// </summary>
    public int? UserId { get; set; }
}
