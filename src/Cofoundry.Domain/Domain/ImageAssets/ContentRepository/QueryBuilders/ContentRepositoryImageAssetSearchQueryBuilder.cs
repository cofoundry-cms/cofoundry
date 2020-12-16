using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryImageAssetSearchQueryBuilder
        : IContentRepositoryImageAssetSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryImageAssetSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PagedQueryResult<ImageAssetSummary>> AsSummaries(SearchImageAssetSummariesQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
