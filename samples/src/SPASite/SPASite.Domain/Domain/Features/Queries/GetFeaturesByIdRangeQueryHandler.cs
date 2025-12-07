namespace SPASite.Domain;

public class GetFeaturesByIdRangeQueryHandler
    : IQueryHandler<GetFeaturesByIdRangeQuery, IReadOnlyDictionary<int, Feature>>
    , IIgnorePermissionCheckHandler
{
    private readonly IContentRepository _contentRepository;

    public GetFeaturesByIdRangeQueryHandler(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task<IReadOnlyDictionary<int, Feature>> ExecuteAsync(GetFeaturesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var features = await _contentRepository
            .CustomEntities()
            .GetByIdRange(query.FeatureIds)
            .AsRenderSummaries()
            .MapItem(MapFeature)
            .ExecuteAsync();

        return features;
    }

    private Feature MapFeature(CustomEntityRenderSummary customEntity)
    {
        var feature = new Feature
        {
            FeatureId = customEntity.CustomEntityId,
            Title = customEntity.Title
        };

        return feature;
    }
}
