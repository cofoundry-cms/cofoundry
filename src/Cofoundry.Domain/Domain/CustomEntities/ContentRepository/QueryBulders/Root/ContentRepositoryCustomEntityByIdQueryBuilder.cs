using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityByIdQueryBuilder
        : IContentRepositoryCustomEntityByIdQueryBuilder
        , IAdvancedContentRepositoryCustomEntityByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _customEntityId;

        public ContentRepositoryCustomEntityByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int customEntityId
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityId = customEntityId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }
        
        public Task<CustomEntityRenderSummary> AsRenderSummaryAsync(PublishStatusQuery? publishStatus = null)
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, publishStatus);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<CustomEntityRenderDetails> AsRenderDetailsAsync(int pageId, PublishStatusQuery? publishStatus = null)
        {
            var query = new GetCustomEntityRenderDetailsByIdQuery()
            {
                CustomEntityId = _customEntityId,
                PageId = pageId
            };
            
            if (publishStatus.HasValue)
            {
                query.PublishStatus = publishStatus.Value;
            }

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<CustomEntityDetails> AsDetailsAsync()
        {
            var query = new GetCustomEntityDetailsByIdQuery(_customEntityId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
        
        public Task<CustomEntityRenderSummary> AsRenderSummaryAsync(int customEntityVersionId)
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, PublishStatusQuery.SpecificVersion);
            query.CustomEntityVersionId = customEntityVersionId;

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<CustomEntityRenderDetails> AsRenderDetailsAsync(int pageId, int customEntityVersionId)
        {
            var query = new GetCustomEntityRenderDetailsByIdQuery()
            {
                CustomEntityId = _customEntityId,
                PageId = pageId,
                PublishStatus = PublishStatusQuery.SpecificVersion,
                CustomEntityVersionId = customEntityVersionId
            };

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        // TODO: YAH

        public Task<PageRoute> AsRouteAsync()
        {
            // TODO: This is probably not required? Is there an exquivalent?
            throw new NotImplementedException();
        }
    }
}
