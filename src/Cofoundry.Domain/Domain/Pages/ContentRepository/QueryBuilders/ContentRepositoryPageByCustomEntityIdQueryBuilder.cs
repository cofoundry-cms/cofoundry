using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageByCustomEntityIdQueryBuilder
    : IAdvancedContentRepositoryPageByCustomEntityIdQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly int _customEntityId;

    public ContentRepositoryPageByCustomEntityIdQueryBuilder(
        IExtendableContentRepository contentRepository,
        int customEntityId
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityId = customEntityId;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<ICollection<PageRoutingInfo>> AsRoutingInfo()
    {
        var query = new GetPageRoutingInfoByCustomEntityIdQuery(_customEntityId);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
