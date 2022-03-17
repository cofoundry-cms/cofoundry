namespace Cofoundry.Domain;

public class SearchPageTemplateSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<PageTemplateSummary>>
{
    public string Name { get; set; }
}
