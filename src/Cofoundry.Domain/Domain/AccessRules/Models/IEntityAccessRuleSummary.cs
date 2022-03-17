namespace Cofoundry.Domain;

/// <summary>
/// Represents the common properties of a summary access rule projection
/// for entities such as Pages and PageDirectories.
/// </summary>
public interface IEntityAccessRuleSummary
{
    /// <summary>
    /// The user area to restrict access to.
    /// </summary>
    public UserAreaMicroSummary UserArea { get; set; }

    /// <summary>
    /// Optionally the rule can be restricted to a specific role
    /// within the specified user area.
    /// </summary>
    public RoleMicroSummary Role { get; set; }
}
