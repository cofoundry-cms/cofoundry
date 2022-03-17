using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageByPathQueryBuilder
    : IContentRepositoryPageByPathQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageByPathQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PageRoutingInfo> AsRoutingInfo(GetPageRoutingInfoByPathQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
