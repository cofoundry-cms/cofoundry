using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageSearchQueryBuilder
        : IContentRepositoryPageSearchQueryBuilder
        , IAdvancedContentRepositoryPageSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PagedQueryResult<PageSummary>> AsSummaries(SearchPageSummariesQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PagedQueryResult<PageRenderSummary>> AsRenderSummaries(SearchPageRenderSummariesQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
