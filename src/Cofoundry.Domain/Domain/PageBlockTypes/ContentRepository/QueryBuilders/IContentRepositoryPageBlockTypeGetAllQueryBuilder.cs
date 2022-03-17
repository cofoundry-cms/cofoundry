namespace Cofoundry.Domain;

/// <summary>
/// Gets all page block types registered in the system.
/// </summary>
public interface IContentRepositoryPageBlockTypeGetAllQueryBuilder
{
    /// <summary>
    /// The PageBlockTypeSummary projection is lightweight and designed to be cacheable.
    /// The results of this query are cached by default.
    /// </summary>
    IDomainRepositoryQueryContext<ICollection<PageBlockTypeSummary>> AsSummaries();
}
