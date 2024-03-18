﻿namespace Cofoundry.Domain;

/// <summary>
/// Adds a new role to a user area with a specific set of permissions.
/// </summary>
public class AddRoleCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// A user friendly title for the role. Role titles must be unique 
    /// per user area and up to 50 characters.
    /// </summary>
    [StringLength(50)]
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Roles must be assigned to an existing user area. The user area code
    /// is 3 characters in length.
    /// </summary>
    [StringLength(3, MinimumLength = 3)]
    [Required]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// The permissions to add the role when it is created.
    /// </summary>
    [ValidateObject]
    public IReadOnlyCollection<PermissionCommandData> Permissions { get; set; } = Array.Empty<PermissionCommandData>();

    /// <summary>
    /// The database id of the newly created role. This is set after the command
    /// has been run.
    /// </summary>
    [OutputValue]
    public int OutputRoleId { get; set; }
}