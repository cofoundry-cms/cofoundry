namespace Cofoundry.Domain;

public class GetPageVersionEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetPageVersionEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetPageVersionEntityMicroSummariesByIdRangeQuery(IEnumerable<int>? pageVersionIds)
        : this(pageVersionIds?.ToArray() ?? [])
    {
    }

    public GetPageVersionEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> pageVersionIds)
    {
        ArgumentNullException.ThrowIfNull(pageVersionIds);

        PageVersionIds = pageVersionIds;
    }

    [Required]
    public IReadOnlyCollection<int> PageVersionIds { get; set; } = Array.Empty<int>();
}
