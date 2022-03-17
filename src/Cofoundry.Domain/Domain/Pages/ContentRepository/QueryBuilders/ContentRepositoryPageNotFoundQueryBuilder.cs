using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageNotFoundQueryBuilder
    : IAdvancedContentRepositoryPageNotFoundQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageNotFoundQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<PageRoute> GetByPath(GetNotFoundPageRouteByPathQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
