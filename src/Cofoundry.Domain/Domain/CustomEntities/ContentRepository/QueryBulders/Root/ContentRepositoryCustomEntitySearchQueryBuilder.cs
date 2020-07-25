using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntitySearchQueryBuilder
        : IContentRepositoryCustomEntitySearchQueryBuilder
        , IAdvancedContentRepositoryCustomEntitySearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntitySearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<PagedQueryResult<CustomEntityRenderSummary>> AsRenderSummaries(SearchCustomEntityRenderSummariesQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PagedQueryResult<CustomEntitySummary>> AsSummaries(SearchCustomEntitySummariesQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
