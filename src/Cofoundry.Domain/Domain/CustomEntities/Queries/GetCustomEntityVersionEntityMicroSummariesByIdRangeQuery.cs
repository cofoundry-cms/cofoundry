namespace Cofoundry.Domain;

public class GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int> ids
        )
        : this(ids.ToList())
    {
    }

    public GetCustomEntityVersionEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> ids
        )
    {
        if (ids == null) throw new ArgumentNullException(nameof(ids));

        CustomEntityVersionIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> CustomEntityVersionIds { get; set; }
}
