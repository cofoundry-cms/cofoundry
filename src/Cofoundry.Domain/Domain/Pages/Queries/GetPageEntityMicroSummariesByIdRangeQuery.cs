namespace Cofoundry.Domain;

public class GetPageEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetPageEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetPageEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int> ids
        )
        : this(ids?.ToList())
    {
    }

    public GetPageEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> ids
        )
    {
        if (ids == null) throw new ArgumentNullException(nameof(ids));

        PageIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> PageIds { get; set; }
}
