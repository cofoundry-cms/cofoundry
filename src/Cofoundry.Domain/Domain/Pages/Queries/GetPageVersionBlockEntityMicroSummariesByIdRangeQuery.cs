namespace Cofoundry.Domain;

public class GetPageVersionBlockEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(IEnumerable<int> pageVersionBlockIds)
        : this(pageVersionBlockIds?.ToList())
    {
    }

    public GetPageVersionBlockEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> pageVersionBlockIds)
    {
        if (pageVersionBlockIds == null) throw new ArgumentNullException(nameof(pageVersionBlockIds));

        PageVersionBlockIds = pageVersionBlockIds;
    }

    [Required]
    public IReadOnlyCollection<int> PageVersionBlockIds { get; set; }
}
