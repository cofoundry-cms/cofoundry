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

        public Task<IDictionary<int, CustomEntityRenderSummary>> AsRenderSummariesAsync(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetCustomEntityRenderSummariesByIdRangeQuery(_customEntityIds);
            if (publishStatus.HasValue)
            {
                query.PublishStatus = publishStatus.Value;
            }

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
        
        public Task<IDictionary<int, CustomEntitySummary>> AsSummariesAsync()
        {
            var query = new GetCustomEntitySummariesByIdRangeQuery(_customEntityIds);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
