namespace Cofoundry.Domain;

public class GetPageDirectoryEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetPageDirectoryEntityMicroSummariesByIdRangeQuery()
    {
        PageDirectoryIds = new List<int>();
    }

    public GetPageDirectoryEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        : this(ids.ToArray() ?? [])
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
