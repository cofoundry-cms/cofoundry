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
        
        public IContentRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary(PublishStatusQuery publishStatus)
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, publishStatus);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary()
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery();
            query.CustomEntityId = _customEntityId;

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<CustomEntityRenderDetails> AsRenderDetails(int pageId, PublishStatusQuery? publishStatus = null)
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

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<CustomEntityDetails> AsDetails()
        {
            var query = new GetCustomEntityDetailsByIdQuery(_customEntityId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary(int customEntityVersionId)
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, PublishStatusQuery.SpecificVersion);
            query.CustomEntityVersionId = customEntityVersionId;

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<CustomEntityRenderDetails> AsRenderDetails(int pageId, int customEntityVersionId)
        {
            var query = new GetCustomEntityRenderDetailsByIdQuery()
            {
                CustomEntityId = _customEntityId,
                PageId = pageId,
                PublishStatus = PublishStatusQuery.SpecificVersion,
                CustomEntityVersionId = customEntityVersionId
            };

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
