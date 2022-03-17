namespace Cofoundry.Domain;

/// <summary>
/// Query that returns detailed information about access restrictions
/// configured for a page, including all access rules as well as those 
/// inherited from parent directories.
/// </summary>>
public class GetPageAccessRuleSetDetailsByPageIdQuery : IQuery<PageAccessRuleSetDetails>
{
    public GetPageAccessRuleSetDetailsByPageIdQuery() { }

    /// <summary>
    /// Initializes the query with the specified <paramref name="pageId"/>.
    /// </summary>
    /// <param name="pageId">
    /// Database id of the page to filter access rules to.
    /// </param>
    public GetPageAccessRuleSetDetailsByPageIdQuery(int pageId)
    {
        PageId = pageId;
    }

    /// <summary>
    /// Database id of the page to filter access rules to.
    /// </summary>
    public int PageId { get; set; }
}
