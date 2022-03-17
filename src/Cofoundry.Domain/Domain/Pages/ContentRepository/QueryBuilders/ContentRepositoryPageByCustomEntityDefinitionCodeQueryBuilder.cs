using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder
    : IAdvancedContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder
    , IExtendableContentRepositoryPart
{
    private readonly string _customEntityDefinitionCode;

    public ContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder(
        IExtendableContentRepository contentRepository,
        string customEntityDefinitionCode
        )
    {
        ExtendableContentRepository = contentRepository;
        _customEntityDefinitionCode = customEntityDefinitionCode;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<ICollection<PageRoute>> AsRoutes()
    {
        var query = new GetPageRoutesByCustomEntityDefinitionCodeQuery(_customEntityDefinitionCode);
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
