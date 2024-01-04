using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityByIdRangeQueryBuilder
    : IContentRepositoryCustomEntityByIdRangeQueryBuilder
    , IAdvancedContentRepositoryCustomEntityByIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<int> _customEntityIds;

    public ContentRepositoryCustomEntityByIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<int> customEntotyIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityIds = customEntotyIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null)
    {
        var query = new GetCustomEntityRenderSummariesByIdRangeQuery(_customEntityIds);
        if (publishStatus.HasValue)
        {
            query.PublishStatus = publishStatus.Value;
        }

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, CustomEntitySummary>> AsSummaries()
    {
        var query = new GetCustomEntitySummariesByIdRangeQuery(_customEntityIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
