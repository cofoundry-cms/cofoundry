using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageGetAllQueryBuilder
    : IContentRepositoryPageGetAllQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryPageGetAllQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<ICollection<PageRoute>> AsRoutes()
    {
        var query = new GetAllPageRoutesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
