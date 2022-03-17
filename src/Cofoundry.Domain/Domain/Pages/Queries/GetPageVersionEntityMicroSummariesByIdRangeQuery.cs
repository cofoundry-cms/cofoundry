namespace Cofoundry.Domain;

public class GetPageVersionEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetPageVersionEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetPageVersionEntityMicroSummariesByIdRangeQuery(IEnumerable<int> pageVersionIds)
        : this(pageVersionIds.ToList())
    {
    }

    public GetPageVersionEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> pageVersionIds)
    {
        if (pageVersionIds == null) throw new ArgumentNullException(nameof(pageVersionIds));

        PageVersionIds = pageVersionIds;
    }

    [Required]
    public IReadOnlyCollection<int> PageVersionIds { get; set; }
}
