using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityByIdRangeQueryBuilder
        : IContentRepositoryCustomEntityByIdRangeQueryBuilder
        , IAdvancedContentRepositoryCustomEntityByIdRangeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly IEnumerable<int> _customEntityIds;

        public ContentRepositoryCustomEntityByIdRangeQueryBuilder(
            IExtendableContentRepository contentRepository,
            IEnumerable<int> customEntotyIds
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityIds = customEntotyIds;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<IDictionary<int, CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetCustomEntityRenderSummariesByIdRangeQuery(_customEntityIds);
            if (publishStatus.HasValue)
            {
                query.PublishStatus = publishStatus.Value;
            }

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
        
        public IContentRepositoryQueryContext<IDictionary<int, CustomEntitySummary>> AsSummaries()
        {
            var query = new GetCustomEntitySummariesByIdRangeQuery(_customEntityIds);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
