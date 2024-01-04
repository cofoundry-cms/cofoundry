namespace Cofoundry.Domain;

/// <summary>
/// Updates an existing role. Also updates the role permission set.
/// </summary>
public class UpdateRoleCommand : IPatchableByIdCommand, ILoggableCommand
{
    /// <summary>
    /// The database id of the role to update.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int RoleId { get; set; }

    /// <summary>
    /// A user friendly title for the role. Role titles must be unique 
    /// per user area and up to 50 characters.
    /// </summary>
    [StringLength(50)]
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The permissions set that the role should be updated to match. 
    /// You must include this otherwise all permissions will be 
    /// removed (unless of course you intend to remove all permissions).
    /// </summary>
    [ValidateObject]
    public IReadOnlyCollection<PermissionCommandData> Permissions { get; set; } = Array.Empty<PermissionCommandData>();
}
