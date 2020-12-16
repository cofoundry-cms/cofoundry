using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
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
}
