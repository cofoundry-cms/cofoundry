using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityByIdQueryBuilder
    : IContentRepositoryCustomEntityByIdQueryBuilder
    , IAdvancedContentRepositoryCustomEntityByIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly int _customEntityId;

    public ContentRepositoryCustomEntityByIdQueryBuilder(
        IExtendableContentRepository contentRepository,
        int customEntityId
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityId = customEntityId;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<CustomEntityRenderSummary?> AsRenderSummary(PublishStatusQuery publishStatusQuery)
    {
        var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, publishStatusQuery);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<CustomEntityRenderSummary?> AsRenderSummary()
    {
        var query = new GetCustomEntityRenderSummaryByIdQuery();
        query.CustomEntityId = _customEntityId;

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<CustomEntityRenderDetails?> AsRenderDetails(int pageId, PublishStatusQuery? publishStatusQuery = null)
    {
        var query = new GetCustomEntityRenderDetailsByIdQuery()
        {
            CustomEntityId = _customEntityId,
            PageId = pageId
        };

        if (publishStatusQuery.HasValue)
        {
            query.PublishStatus = publishStatusQuery.Value;
        }

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<CustomEntityDetails?> AsDetails()
    {
        var query = new GetCustomEntityDetailsByIdQuery(_customEntityId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<CustomEntityRenderSummary?> AsRenderSummary(int customEntityVersionId)
    {
        var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, PublishStatusQuery.SpecificVersion)
        {
            CustomEntityVersionId = customEntityVersionId
        };

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<CustomEntityRenderDetails?> AsRenderDetails(int pageId, int customEntityVersionId)
    {
        var query = new GetCustomEntityRenderDetailsByIdQuery()
        {
            CustomEntityId = _customEntityId,
            PageId = pageId,
            PublishStatus = PublishStatusQuery.SpecificVersion,
            CustomEntityVersionId = customEntityVersionId
        };

        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
