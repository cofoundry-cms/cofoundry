using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetAllFeaturesQueryHandler
        : IQueryHandler<GetAllFeaturesQuery, ICollection<Feature>>
        , IIgnorePermissionCheckHandler
    {
        private readonly IContentRepository _contentRepository;

        public GetAllFeaturesQueryHandler(
            IContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        public async Task<ICollection<Feature>> ExecuteAsync(GetAllFeaturesQuery query, IExecutionContext executionContext)
        {
            var features = await _contentRepository
                .CustomEntities()
                .GetByDefinition<FeatureCustomEntityDefinition>()
                .AsRenderSummary()
                .MapItem(MapFeature)
                .ExecuteAsync();

            return features;
        }

        private Feature MapFeature(CustomEntityRenderSummary customEntity)
        {
            var feature = new Feature();

            feature.FeatureId = customEntity.CustomEntityId;
            feature.Title = customEntity.Title;

            return feature;
        }
    }
}
