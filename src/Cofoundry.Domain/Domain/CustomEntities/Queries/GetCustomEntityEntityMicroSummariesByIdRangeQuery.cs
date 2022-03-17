namespace Cofoundry.Domain;

public class GetCustomEntityEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetCustomEntityEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetCustomEntityEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int> customEntityIds
        )
        : this(customEntityIds?.ToList())
    {
    }

    public GetCustomEntityEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> customEntityIds
        )
    {
        if (customEntityIds == null) throw new ArgumentNullException(nameof(customEntityIds));

        CustomEntityIds = customEntityIds;
    }

    [Required]
    public IReadOnlyCollection<int> CustomEntityIds { get; set; }
}
