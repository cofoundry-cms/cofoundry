namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class SearchErrorSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<ErrorSummary>>
{
    public string? Text { get; set; }
}
