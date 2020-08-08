using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<PagedQueryResult<CustomEntityRenderSummary>> AsRenderSummaries(SearchCustomEntityRenderSummariesQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PagedQueryResult<CustomEntitySummary>> AsSummaries(SearchCustomEntitySummariesQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
