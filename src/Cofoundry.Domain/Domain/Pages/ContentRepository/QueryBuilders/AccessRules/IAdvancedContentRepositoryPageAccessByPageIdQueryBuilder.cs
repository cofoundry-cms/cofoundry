namespace Cofoundry.Domain;

/// <summary>
/// Queries for retrieving page access data by the id of the parent page.
/// </summary>
public interface IAdvancedContentRepositoryPageAccessByPageIdQueryBuilder
{
    /// <summary>
    /// Query that returns detailed information about access restrictions
    /// configured for a page, including all access rules as well as those 
    /// inherited from parent directories.
    /// </summary>>
    IDomainRepositoryQueryContext<PageAccessRuleSetDetails> AsDetails();
}
