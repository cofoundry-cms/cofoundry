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
        if (ids == null) throw new ArgumentNullException(nameof(ids));

        ImageAssetIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> ImageAssetIds { get; set; }
}
