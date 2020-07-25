using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<PagedQueryResult<PageSummary>> AsSummaries(SearchPageSummariesQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PagedQueryResult<PageRenderSummary>> AsRenderSummaries(SearchPageRenderSummariesQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
