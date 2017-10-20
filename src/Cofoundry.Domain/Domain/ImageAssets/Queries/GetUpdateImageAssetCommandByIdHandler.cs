using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetUpdateImageAssetCommandByIdHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateImageAssetCommand>, UpdateImageAssetCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdateImageAssetCommand>, UpdateImageAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdateImageAssetCommandByIdHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<UpdateImageAssetCommand> ExecuteAsync(GetByIdQuery<UpdateImageAssetCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .ImageAssets
                .Include(a => a.ImageAssetTags)
                .ThenInclude(a => a.Tag)
                .AsNoTracking()
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            var result = new UpdateImageAssetCommand()
            {
                ImageAssetId = dbResult.ImageAssetId,
                DefaultAnchorLocation = dbResult.DefaultAnchorLocation,
                Title = dbResult.FileDescription
            };

            result.Tags = dbResult
                    .ImageAssetTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t)
                    .ToArray();

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdateImageAssetCommand> query)
        {
            yield return new ImageAssetUpdatePermission();
        }

        #endregion
    }
}
