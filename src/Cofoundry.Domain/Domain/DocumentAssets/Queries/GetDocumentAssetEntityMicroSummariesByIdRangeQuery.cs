namespace Cofoundry.Domain;

public class GetDocumentAssetEntityMicroSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    public GetDocumentAssetEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetDocumentAssetEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int> ids
        )
        : this(ids?.ToArray() ?? [])
    {
    }

    public GetDocumentAssetEntityMicroSummariesByIdRangeQuery(
        IReadOnlyCollection<int> ids
        )
    {
        ArgumentNullException.ThrowIfNull(ids);

        DocumentAssetIds = ids;
    }

    [Required]
    public IReadOnlyCollection<int> DocumentAssetIds { get; set; } = Array.Empty<int>();
}
