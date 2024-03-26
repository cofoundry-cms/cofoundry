namespace Cofoundry.Domain;

public class GetPageVersionBlockEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery()
    {
        PageVersionBlockIds = new List<int>();
    }

    public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(IEnumerable<int> pageVersionBlockIds)
        : this(pageVersionBlockIds?.ToArray() ?? [])
    {
    }

    public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> pageVersionBlockIds)
    {
        ArgumentNullException.ThrowIfNull(pageVersionBlockIds);

        PageVersionBlockIds = pageVersionBlockIds;
    }

    [Required]
    public IReadOnlyCollection<int> PageVersionBlockIds { get; set; }
}
