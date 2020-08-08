using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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
        
        public IDomainRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary(PublishStatusQuery publishStatus)
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, publishStatus);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary()
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery();
            query.CustomEntityId = _customEntityId;

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<CustomEntityRenderDetails> AsRenderDetails(int pageId, PublishStatusQuery? publishStatus = null)
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

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<CustomEntityDetails> AsDetails()
        {
            var query = new GetCustomEntityDetailsByIdQuery(_customEntityId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary(int customEntityVersionId)
        {
            var query = new GetCustomEntityRenderSummaryByIdQuery(_customEntityId, PublishStatusQuery.SpecificVersion);
            query.CustomEntityVersionId = customEntityVersionId;

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<CustomEntityRenderDetails> AsRenderDetails(int pageId, int customEntityVersionId)
        {
            var query = new GetCustomEntityRenderDetailsByIdQuery()
            {
                CustomEntityId = _customEntityId,
                PageId = pageId,
                PublishStatus = PublishStatusQuery.SpecificVersion,
                CustomEntityVersionId = customEntityVersionId
            };

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
