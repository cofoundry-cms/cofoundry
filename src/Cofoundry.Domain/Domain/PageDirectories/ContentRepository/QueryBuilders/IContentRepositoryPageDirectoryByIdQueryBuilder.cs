namespace Cofoundry.Domain;

/// <summary>
/// Queries for retrieving page directory data using a unique database id.
/// </summary>
public interface IContentRepositoryPageDirectoryByIdQueryBuilder
{
    /// <summary>
    /// The PageDirectoryRoute projection is used in dynamic page routing and is designed to
    /// be lightweight and cacheable. The results of this query are cached by default.
    /// </summary>
    IDomainRepositoryQueryContext<PageDirectoryRoute> AsRoute();
}
