namespace SPASite.Domain;

public class SearchCatSummariesQuery
    : SimplePageableQuery
    , IQuery<PagedQueryResult<CatSummary>>
{
}
