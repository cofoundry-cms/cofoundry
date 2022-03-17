namespace Cofoundry.Domain;

public class GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        : this(ids.ToList())
    {
    }

    public GetCustomEntityVersionPageBlockEntityMicroSummariesByIdRangeQuery(IReadOnlyCollection<int> ids)
    {
        if (ids == null) throw new ArgumentNullException(nameof(ids));

        CustomEntityVersionPageBlockIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> CustomEntityVersionPageBlockIds { get; set; }
}
