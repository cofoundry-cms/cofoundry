using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder
    : IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<ICollection<ICustomEntityRoutingRule>> AsRoutingRules()
    {
        var query = new GetAllCustomEntityRoutingRulesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
