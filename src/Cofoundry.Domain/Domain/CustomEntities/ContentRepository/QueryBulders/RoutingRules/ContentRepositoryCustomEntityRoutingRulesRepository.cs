using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryCustomEntityRoutingRulesRepository
        : IAdvancedContentRepositoryCustomEntityRoutingRulesRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryCustomEntityRoutingRulesRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder GetAll()
    {
        return new ContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder(ExtendableContentRepository);
    }

    public IAdvancedContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder GetByRouteFormat(string routeFormat)
    {
        return new ContentRepositoryCustomEntityRoutingRuleByRouteFormatQueryBuilder(ExtendableContentRepository, routeFormat);
    }
}
