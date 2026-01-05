namespace SPASite.Domain;

public class GetAllFeaturesQueryHandler
    : IQueryHandler<GetAllFeaturesQuery, IReadOnlyCollection<Feature>>
    , IIgnorePermissionCheckHandler
{
    private readonly IContentRepository _contentRepository;

    public GetAllFeaturesQueryHandler(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task<IReadOnlyCollection<Feature>> ExecuteAsync(GetAllFeaturesQuery query, IExecutionContext executionContext)
    {
        var features = await _contentRepository
            .CustomEntities()
            .GetByDefinition<FeatureCustomEntityDefinition>()
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
