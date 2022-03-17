using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryRewriteRuleGetAllQueryBuilder
    : IContentRepositoryRewriteRuleGetAllQueryBuilder
    , IExtendableContentRepositoryPart
{
    public ContentRepositoryRewriteRuleGetAllQueryBuilder(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<ICollection<RewriteRuleSummary>> AsSummaries()
    {
        var query = new GetAllRewriteRuleSummariesQuery();
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }
}
