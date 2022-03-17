using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageByCustomEntityIdRangeQueryBuilder
    : IAdvancedContentRepositoryPageByCustomEntityIdRangeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly IEnumerable<int> _customEntityIds;

    public ContentRepositoryPageByCustomEntityIdRangeQueryBuilder(
        IExtendableContentRepository contentRepository,
        IEnumerable<int> customEntityIds
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityIds = customEntityIds;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<IDictionary<int, ICollection<PageRoutingInfo>>> AsRoutingInfo()
    {
        var query = new GetPageRoutingInfoByCustomEntityIdRangeQuery(_customEntityIds);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
