namespace Cofoundry.Domain;

/// <summary>
/// Validates a username, returning any errors found. By default the validator checks that 
/// the format contains only the characters permitted by the <see cref="UsernameOptions"/> 
/// configuration settings, as well as checking for uniquness.
/// </summary>
public class ValidateUsernameQuery : IQuery<ValidationQueryResult>
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to check.
    /// This is required because formatting can be different for each user area.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; }

    /// <summary>
    /// The username to check for uniqueness. The username will
    /// be normalized before the check is made so there is no need
    /// to pre-format this value.
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// Database id of an existing user. If the user is new then this can be <see langword="null"/>.
    /// </summary>
    public int? UserId { get; set; }
}
