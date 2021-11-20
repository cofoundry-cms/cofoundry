using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Samples.SPASite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetCatDetailsByIdQueryHandler
        : IQueryHandler<GetCatDetailsByIdQuery, CatDetails>
        , IIgnorePermissionCheckHandler
    {
        private readonly IContentRepository _contentRepository;
        private readonly SPASiteDbContext _dbContext;

        public GetCatDetailsByIdQueryHandler(
            IContentRepository contentRepository,
            SPASiteDbContext dbContext
            )
        {
            _contentRepository = contentRepository;
            _dbContext = dbContext;
        }

        public async Task<CatDetails> ExecuteAsync(GetCatDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var cat = await _contentRepository
                .CustomEntities()
                .GetById(query.CatId)
                .AsRenderSummary()
                .Map(MapCatAsync)
                .ExecuteAsync();

            return cat;
        }

        private async Task<CatDetails> MapCatAsync(CustomEntityRenderSummary customEntity)
        {
            var model = customEntity.Model as CatDataModel;
            var cat = new CatDetails();

            cat.CatId = customEntity.CustomEntityId;
            cat.Name = customEntity.Title;
            cat.Description = model.Description;
            cat.Breed = await GetBreedAsync(model.BreedId);
            cat.Features = await GetFeaturesAsync(model.FeatureIds);
            cat.Images = await GetImagesAsync(model.ImageAssetIds);
            cat.TotalLikes = await GetLikeCount(customEntity.CustomEntityId);

            return cat;
        }

        private Task<int> GetLikeCount(int catId)
        {
            return _dbContext
                .CatLikeCounts
                .AsNoTracking()
                .Where(c => c.CatCustomEntityId == catId)
                .Select(c => c.TotalLikes)
                .FirstOrDefaultAsync();
        }

        private async Task<Breed> GetBreedAsync(int? breedId)
        {
            if (!breedId.HasValue) return null;
            var query = new GetBreedByIdQuery(breedId.Value);

            return await _contentRepository.ExecuteQueryAsync(query);
        }

        private async Task<ICollection<Feature>> GetFeaturesAsync(ICollection<int> featureIds)
        {
            if (EnumerableHelper.IsNullOrEmpty(featureIds)) return Array.Empty<Feature>();
            var query = new GetFeaturesByIdRangeQuery(featureIds);

            var features = await _contentRepository.ExecuteQueryAsync(query);

            return features
                .Select(f => f.Value)
                .OrderBy(f => f.Title)
                .ToList();
        }

        private async Task<ICollection<ImageAssetRenderDetails>> GetImagesAsync(ICollection<int> imageAssetIds)
        {
            if (EnumerableHelper.IsNullOrEmpty(imageAssetIds)) return Array.Empty<ImageAssetRenderDetails>();

            var images = await _contentRepository
                .ImageAssets()
                .GetByIdRange(imageAssetIds)
                .AsRenderDetails()
                .FilterAndOrderByKeys(imageAssetIds)
                .ExecuteAsync();

            return images;
        }
    }
}
