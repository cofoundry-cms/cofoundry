namespace Cofoundry.Domain;

/// <summary>
/// Queries for returning all page directories.
/// </summary>
public interface IContentRepositoryPageDirectoryGetAllQueryBuilder
{
    /// <summary>
    /// The PageDirectoryRoute projection is used in dynamic page routing and is designed to
    /// be lightweight and cacheable.
    /// </summary>
    IDomainRepositoryQueryContext<ICollection<PageDirectoryRoute>> AsRoutes();
}
