namespace Cofoundry.Domain;

/// <summary>
/// Contains information about a related entity and
/// whether the relationship can be safely removed.
/// An entity dependency is typically becuase one entity
/// references another in an unstructured data blob where 
/// the relationship cannot be enforced by the database.
/// </summary>
public class EntityDependencySummary
{
    /// <summary>
    /// An entity that is related to the entity being queried.
    /// </summary>
    public RootEntityMicroSummary Entity { get; set; }

    /// <summary>
    /// Indicates whether the relationship to this entity can be 
    /// removed. This happens if the entity is used in a required
    /// property.
    /// </summary>
    public bool CanDelete { get; set; }
}
