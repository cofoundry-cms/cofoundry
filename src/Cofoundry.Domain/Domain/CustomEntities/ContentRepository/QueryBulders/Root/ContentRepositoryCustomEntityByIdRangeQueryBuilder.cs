using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<IDictionary<int, CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetCustomEntityRenderSummariesByIdRangeQuery(_customEntityIds);
            if (publishStatus.HasValue)
            {
                query.PublishStatus = publishStatus.Value;
            }

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
        
        public IDomainRepositoryQueryContext<IDictionary<int, CustomEntitySummary>> AsSummaries()
        {
            var query = new GetCustomEntitySummariesByIdRangeQuery(_customEntityIds);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
