namespace Cofoundry.Domain;

public class GetDocumentAssetEntityMicroSummariesByIdRangeQuery : IQuery<IDictionary<int, RootEntityMicroSummary>>
{
    public GetDocumentAssetEntityMicroSummariesByIdRangeQuery()
    {
    }

    public GetDocumentAssetEntityMicroSummariesByIdRangeQuery(
        IEnumerable<int> ids
        )
        : this(ids?.ToList())
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
    public IReadOnlyCollection<int> DocumentAssetIds { get; set; }
}
