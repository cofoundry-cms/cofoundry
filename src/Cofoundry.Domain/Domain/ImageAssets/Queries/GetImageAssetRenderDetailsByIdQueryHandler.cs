using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetImageAssetRenderDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<ImageAssetRenderDetails>, ImageAssetRenderDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<ImageAssetRenderDetails>, ImageAssetRenderDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;

        public GetImageAssetRenderDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
        }

        #endregion

        #region execution

        public async Task<ImageAssetRenderDetails> ExecuteAsync(GetByIdQuery<ImageAssetRenderDetails> query, IExecutionContext executionContext)
        {
            var asset = await _imageAssetCache.GetOrAddAsync(query.Id, () =>
            {
                var result = Query(query.Id).SingleOrDefaultAsync();
                return result;
            });

            return asset;
        }

        private IQueryable<ImageAssetRenderDetails> Query(int id)
        {
            return _dbContext
                .ImageAssets
                .AsNoTracking()
                .FilterById(id)
                .ProjectTo<ImageAssetRenderDetails>();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<ImageAssetRenderDetails> query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
