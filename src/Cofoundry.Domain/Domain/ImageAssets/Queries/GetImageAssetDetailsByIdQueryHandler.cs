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
    public class GetImageAssetDetailsByIdQueryHandler 
        : IQueryHandler<GetImageAssetDetailsByIdQuery, ImageAssetDetails>
        , IPermissionRestrictedQueryHandler<GetImageAssetDetailsByIdQuery, ImageAssetDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetDetailsMapper _imageAssetDetailsMapper;

        public GetImageAssetDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetDetailsMapper imageAssetDetailsMapper
            )
        {
            _dbContext = dbContext;
            _imageAssetDetailsMapper = imageAssetDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<ImageAssetDetails> ExecuteAsync(GetImageAssetDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .ImageAssets
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.Updater)
                .Include(i => i.ImageAssetTags)
                .ThenInclude(i => i.Tag)
                .FilterById(query.ImageAssetId)
                .SingleOrDefaultAsync();

            var result = _imageAssetDetailsMapper.Map(dbResult);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetDetailsByIdQuery query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
