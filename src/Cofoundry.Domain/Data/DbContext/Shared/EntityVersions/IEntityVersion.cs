namespace Cofoundry.Domain.Data;

/// <summary>
/// Abstraction of data required for an entity versioning table e.g. for
/// PageVersion or CustomEntityVersion.
/// </summary>
public interface IEntityVersion : ICreateable
{
    /// <summary>
    /// Mapped from the domain <see cref="WorkFlowStatus"/> enum, this is the workflow 
    /// state of this version e.g. draft/published.
    /// </summary>
    int WorkFlowStatusId { get; set; }
}
