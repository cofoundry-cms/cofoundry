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
        ArgumentNullException.ThrowIfNull(ids);

        PageIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> PageIds { get; set; }
}
