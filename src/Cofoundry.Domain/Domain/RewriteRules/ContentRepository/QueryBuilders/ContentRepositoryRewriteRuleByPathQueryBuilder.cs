using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<RewriteRuleSummary> AsSummary()
        {
            var query = new GetRewriteRuleSummaryByPathQuery() { Path = _path };
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
