namespace Cofoundry.Domain.Data;

/// <summary>
/// Users can be partitioned into different 'User Areas' that enabled the identity system use by the Cofoundry administration area 
/// to be reused for other purposes, but this isn't a common scenario and often there will only be the Cofoundry UserArea. UserAreas
/// are defined in code by defining an <see cref="IUserAreaDefinition"/>.
/// </summary>
public class UserArea
{
    /// <summary>
    /// 3 character code and primary key.
    /// </summary>
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// A human readable name used to describe the user area.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Users attached to this user area. A User can only be attached to one user area.
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Dynamic website routes can optionally be restircted to specific user areas. This
    /// collection references zero or more access rules at the <see cref="Page"/> level.
    /// </summary>
    public ICollection<PageAccessRule> PageAccessRules { get; set; } = new List<PageAccessRule>();

    /// <summary>
    /// Dynamic website routes can optionally be restircted to specific user areas. This
    /// collection references zero or more access rules at the <see cref="PageDirectory"/> level.
    /// </summary>
    public ICollection<PageDirectoryAccessRule> PageDirectoryAccessRules { get; set; } = new List<PageDirectoryAccessRule>();
}
