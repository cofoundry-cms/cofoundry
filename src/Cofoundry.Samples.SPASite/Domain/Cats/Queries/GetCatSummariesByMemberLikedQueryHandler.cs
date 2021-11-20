using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Samples.SPASite.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetCatSummariesByMemberLikedQueryHandler
        : IQueryHandler<GetCatSummariesByMemberLikedQuery, ICollection<CatSummary>>
        , ILoggedInPermissionCheckHandler
    {
        private readonly IContentRepository _contentRepository;
        private readonly SPASiteDbContext _dbContext;

        public GetCatSummariesByMemberLikedQueryHandler(
            IContentRepository contentRepository,
            SPASiteDbContext dbContext
            )
        {
            _contentRepository = contentRepository;
            _dbContext = dbContext;
        }

        public async Task<ICollection<CatSummary>> ExecuteAsync(GetCatSummariesByMemberLikedQuery query, IExecutionContext executionContext)
        {
            var userCatIds = await _dbContext
                .CatLikes
                .AsNoTracking()
                .Where(c => c.UserId == query.UserId)
                .OrderByDescending(c => c.CreateDate)
                .Select(c => c.CatCustomEntityId)
                .ToListAsync();

            // GetByIdRange queries return a dictionary to make lookups easier, so we 
            // have an extra step to do if we want to set the ordering to match the
            // original id collection.
            var catCustomEntities = await _contentRepository
                .CustomEntities()
                .GetByIdRange(userCatIds)
                .AsRenderSummaries()
                .FilterAndOrderByKeys(userCatIds)
                .ExecuteAsync();

            var allMainImages = await GetMainImages(catCustomEntities);
            var allLikeCounts = await GetLikeCounts(catCustomEntities);

            return MapCats(catCustomEntities, allMainImages, allLikeCounts);
        }

        private Task<IDictionary<int, ImageAssetRenderDetails>> GetMainImages(ICollection<CustomEntityRenderSummary> customEntities)
        {
            var imageAssetIds = customEntities
                .Select(i => (CatDataModel)i.Model)
                .Where(m => !EnumerableHelper.IsNullOrEmpty(m.ImageAssetIds))
                .Select(m => m.ImageAssetIds.First())
                .Distinct();

            return _contentRepository
                .ImageAssets()
                .GetByIdRange(imageAssetIds)
                .AsRenderDetails()
                .ExecuteAsync();
        }

        private Task<Dictionary<int, int>> GetLikeCounts(ICollection<CustomEntityRenderSummary> customEntities)
        {
            var catIds = customEntities
                .Select(i => i.CustomEntityId)
                .Distinct()
                .ToList();

            return _dbContext
                .CatLikeCounts
                .AsNoTracking()
                .Where(c => catIds.Contains(c.CatCustomEntityId))
                .ToDictionaryAsync(c => c.CatCustomEntityId, c => c.TotalLikes);
        }

        private List<CatSummary> MapCats(
            ICollection<CustomEntityRenderSummary> customEntities,
            IDictionary<int, ImageAssetRenderDetails> images,
            IDictionary<int, int> allLikeCounts
            )
        {
            var cats = new List<CatSummary>(customEntities.Count());

            foreach (var customEntity in customEntities)
            {
                var model = (CatDataModel)customEntity.Model;

                var cat = new CatSummary();
                cat.CatId = customEntity.CustomEntityId;
                cat.Name = customEntity.Title;
                cat.Description = model.Description;
                cat.TotalLikes = allLikeCounts.GetOrDefault(customEntity.CustomEntityId);

                if (!EnumerableHelper.IsNullOrEmpty(model.ImageAssetIds))
                {
                    cat.MainImage = images.GetOrDefault(model.ImageAssetIds.FirstOrDefault());
                }

                cats.Add(cat);
            }

            return cats;
        }
    }
}
