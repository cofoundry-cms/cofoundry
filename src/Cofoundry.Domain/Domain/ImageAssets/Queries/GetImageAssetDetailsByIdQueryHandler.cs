using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetImageAssetDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<ImageAssetDetails>, ImageAssetDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<ImageAssetDetails>, ImageAssetDetails>
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

        public async Task<ImageAssetDetails> ExecuteAsync(GetByIdQuery<ImageAssetDetails> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .ImageAssets
                .AsNoTracking()
                .Include(i => i.Creator)
                .Include(i => i.Updater)
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            var result = _imageAssetDetailsMapper.Map(dbResult);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<ImageAssetDetails> query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
