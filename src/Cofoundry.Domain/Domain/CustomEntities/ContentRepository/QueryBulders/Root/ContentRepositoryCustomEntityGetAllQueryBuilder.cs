using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityGetAllQueryBuilder
        : IContentRepositoryCustomEntityGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _customEntityDefinitionCode;

        public ContentRepositoryCustomEntityGetAllQueryBuilder(
            IExtendableContentRepository contentRepository,
            string customEntityDefinitionCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCode = customEntityDefinitionCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<CustomEntityRenderSummary>> AsRenderSummaryAsync(PublishStatusQuery? publishStatusQuery = null)
        {
            var query = new GetCustomEntityRenderSummariesByDefinitionCodeQuery(_customEntityDefinitionCode);

            if (publishStatusQuery.HasValue)
            {
                query.PublishStatus = publishStatusQuery.Value;
            }

            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<ICollection<CustomEntityRoute>> AsRoutesAsync()
        {
            var query = new GetCustomEntityRoutesByDefinitionCodeQuery(_customEntityDefinitionCode);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
