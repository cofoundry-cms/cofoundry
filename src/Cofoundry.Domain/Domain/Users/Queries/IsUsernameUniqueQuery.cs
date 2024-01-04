namespace Cofoundry.Domain;

/// <summary>
/// Determines if a username is unique within a specific user area.
/// Usernames only have to be unique per user area.
/// </summary>
public class IsUsernameUniqueQuery : IQuery<bool>
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to check.
    /// This is required because usernames only have to be unique per user area.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// The username to check for uniqueness. The username will
    /// be "uniquified" before the check is made so there is no need
    /// to pre-format this value. Null or empty values are not valid 
    /// but will return <see langword="true"/> because although uniqueness 
    /// validation should not be triggered for these values it is technically 
    /// the correct answer.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Optional database id of an existing user to exclude from the uniqueness 
    /// check. Use this when checking the uniqueness of an existing user.
    /// </summary>
    public int? UserId { get; set; }
}
