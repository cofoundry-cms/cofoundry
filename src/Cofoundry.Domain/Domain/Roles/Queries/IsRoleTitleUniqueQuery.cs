﻿namespace Cofoundry.Domain;

/// <summary>
/// Determines if a role title is unique within a specific UserArea.
/// Role titles only have to be unique per UserArea.
/// </summary>
public class IsRoleTitleUniqueQuery : IQuery<bool>
{
    /// <summary>
    /// Optional database id of an existing role to exclude from the uniqueness 
    /// check. Use this when checking the uniqueness of an existing Role.
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// Role titles only have to be unique per UserArea.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// The role title to check for uniqueness (not case sensitive).
    /// Null or empty values are not valid, but will return <see langword="true"/> 
    /// because although uniqueness validation should not be triggered for these values
    /// it is technically the correct answer.
    /// </summary>
    public string? Title { get; set; }
}
