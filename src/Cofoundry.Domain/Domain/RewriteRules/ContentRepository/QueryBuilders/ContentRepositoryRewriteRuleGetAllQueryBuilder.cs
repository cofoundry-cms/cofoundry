using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<ICollection<RewriteRuleSummary>> AsSummaries()
        {
            var query = new GetAllRewriteRuleSummariesQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
