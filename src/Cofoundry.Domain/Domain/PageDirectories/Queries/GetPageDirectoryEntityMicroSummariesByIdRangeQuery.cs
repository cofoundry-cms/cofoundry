namespace Cofoundry.Domain;

public class GetPageDirectoryEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetPageDirectoryEntityMicroSummariesByIdRangeQuery() { }

    public GetPageDirectoryEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        : this(ids.ToList())
    {
    }

    public GetPageDirectoryEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        PageDirectoryIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> PageDirectoryIds { get; set; }
}
