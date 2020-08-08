using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetImageAssetRenderDetailsByIdQueryHandler 
        : IQueryHandler<GetImageAssetRenderDetailsByIdQuery, ImageAssetRenderDetails>
        , IPermissionRestrictedQueryHandler<GetImageAssetRenderDetailsByIdQuery, ImageAssetRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly IImageAssetRenderDetailsMapper _imageAssetRenderDetailsMapper;

        public GetImageAssetRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache,
            IImageAssetRenderDetailsMapper imageAssetRenderDetailsMapper
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
            _imageAssetRenderDetailsMapper = imageAssetRenderDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<ImageAssetRenderDetails> ExecuteAsync(GetImageAssetRenderDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var asset = await _imageAssetCache.GetOrAddAsync(query.ImageAssetId, async () =>
            {
                var dbResult = await Query(query.ImageAssetId).SingleOrDefaultAsync();
                var result = _imageAssetRenderDetailsMapper.Map(dbResult);
                return result;
            });

            return asset;
        }

        private IQueryable<ImageAsset> Query(int id)
        {
            return _dbContext
                .ImageAssets
                .AsNoTracking()
                .FilterById(id);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetRenderDetailsByIdQuery query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
