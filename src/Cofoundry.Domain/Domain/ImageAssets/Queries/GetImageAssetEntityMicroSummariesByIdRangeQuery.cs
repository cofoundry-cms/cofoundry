namespace Cofoundry.Domain;

public class GetImageAssetEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetImageAssetEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetImageAssetEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int> ids
        )
        : this(ids?.ToList())
    {
    }

    public GetImageAssetEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> ids
        )
    {
        ArgumentNullException.ThrowIfNull(ids);

        ImageAssetIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> ImageAssetIds { get; set; }
}
