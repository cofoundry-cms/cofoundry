using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityGetByDefinitionQueryBuilder
        : IContentRepositoryCustomEntityGetByDefinitionQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _customEntityDefinitionCode;

        public ContentRepositoryCustomEntityGetByDefinitionQueryBuilder(
            IExtendableContentRepository contentRepository,
            string customEntityDefinitionCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCode = customEntityDefinitionCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<ICollection<CustomEntityRenderSummary>> AsRenderSummary(PublishStatusQuery? publishStatusQuery = null)
        {
            var query = new GetCustomEntityRenderSummariesByDefinitionCodeQuery(_customEntityDefinitionCode);

            if (publishStatusQuery.HasValue)
            {
                query.PublishStatus = publishStatusQuery.Value;
            }

            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<ICollection<CustomEntityRoute>> AsRoutes()
        {
            var query = new GetCustomEntityRoutesByDefinitionCodeQuery(_customEntityDefinitionCode);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
