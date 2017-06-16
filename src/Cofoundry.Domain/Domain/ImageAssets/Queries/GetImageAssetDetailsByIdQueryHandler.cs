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
    public class GetImageAssetDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<ImageAssetDetails>, ImageAssetDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<ImageAssetDetails>, ImageAssetDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetImageAssetDetailsByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<ImageAssetDetails> ExecuteAsync(GetByIdQuery<ImageAssetDetails> query, IExecutionContext executionContext)
        {
            var result = await _dbContext
                .ImageAssets
                .AsNoTracking()
                .FilterById(query.Id)
                .ProjectTo<ImageAssetDetails>()
                .SingleOrDefaultAsync();

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
