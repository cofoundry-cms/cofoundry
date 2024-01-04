using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageByIdRangeQueryBuilder
    : IContentRepositoryPageByIdRangeQueryBuilder
    , IAdvancedContentRepositoryPageByIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<int> _pageIds;

    public ContentRepositoryPageByIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<int> pageIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _pageIds = pageIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, PageRoute>> AsRoutes()
    {
        var query = new GetPageRoutesByIdRangeQuery(_pageIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, PageRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null)
    {
        var query = new GetPageRenderSummariesByIdRangeQuery(_pageIds, publishStatus);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, PageRenderDetails>> AsRenderDetails(PublishStatusQuery? publishStatus = null)
    {
        var query = new GetPageRenderDetailsByIdRangeQuery(_pageIds, publishStatus);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public IDomainRepositoryQueryContext<IReadOnlyDictionary<int, PageSummary>> AsSummaries()
    {
        var query = new GetPageSummariesByIdRangeQuery(_pageIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
