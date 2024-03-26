namespace Cofoundry.Domain;

public class GetPageEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetPageEntityMicroSummariesByIdRangeQuery()
    {
        PageIds = new List<int>();
    }

    public GetPageEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int>? ids
        )
        : this(ids?.ToArray() ?? [])
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
