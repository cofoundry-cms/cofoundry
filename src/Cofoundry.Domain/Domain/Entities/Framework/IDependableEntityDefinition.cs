namespace Cofoundry.Domain;

/// <summary>
/// Indicates that an entity can be a dependecy to other entities via
/// a non-database level relation e.g. in a data model persisted as
/// an unstructred data blob.
/// </summary>
public interface IDependableEntityDefinition : IEntityDefinition
{
    /// <summary>
    /// Returns a query that will get information about the aggregate root associated with entity
    /// ids provided e.g. for an Image Asset the aggregate root is an Image AssetAsset, but for a PageVersion, the 
    /// aggregate root is the Page.
    /// </summary>
    /// <param name="ids">Ids of the entities to get.</param>
    IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids);
}
