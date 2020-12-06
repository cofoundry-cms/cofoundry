using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityByDefinitionQueryBuilder
        : IContentRepositoryCustomEntityByDefinitionQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _customEntityDefinitionCode;

        public ContentRepositoryCustomEntityByDefinitionQueryBuilder(
            IExtendableContentRepository contentRepository,
            string customEntityDefinitionCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCode = customEntityDefinitionCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ICollection<CustomEntityRenderSummary>> AsRenderSummary(PublishStatusQuery? publishStatusQuery = null)
        {
            var query = new GetCustomEntityRenderSummariesByDefinitionCodeQuery(_customEntityDefinitionCode);

            if (publishStatusQuery.HasValue)
            {
                query.PublishStatus = publishStatusQuery.Value;
            }

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<ICollection<CustomEntityRoute>> AsRoutes()
        {
            var query = new GetCustomEntityRoutesByDefinitionCodeQuery(_customEntityDefinitionCode);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
