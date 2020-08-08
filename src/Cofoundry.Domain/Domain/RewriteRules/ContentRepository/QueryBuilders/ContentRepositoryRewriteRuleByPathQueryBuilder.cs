using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryRewriteRuleByPathQueryBuilder
        : IContentRepositoryRewriteRuleByPathQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _path;

        public ContentRepositoryRewriteRuleByPathQueryBuilder(
            IExtendableContentRepository contentRepository,
            string path
            )
        {
            ExtendableContentRepository = contentRepository;
            _path = path;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<RewriteRuleSummary> AsSummary()
        {
            var query = new GetRewriteRuleSummaryByPathQuery() { Path = _path };
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
