namespace Cofoundry.Domain;

public class GetImageAssetEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetImageAssetEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetImageAssetEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int>? ids
        )
        : this(ids?.ToArray() ?? [])
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
    public IReadOnlyCollection<int> ImageAssetIds { get; set; } = Array.Empty<int>();
}
