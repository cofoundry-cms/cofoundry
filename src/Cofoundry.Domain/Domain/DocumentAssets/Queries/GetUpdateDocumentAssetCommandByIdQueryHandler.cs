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
    public class GetUpdateDocumentAssetCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateDocumentAssetCommand>, UpdateDocumentAssetCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdateDocumentAssetCommand>, UpdateDocumentAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdateDocumentAssetCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<UpdateDocumentAssetCommand> ExecuteAsync(GetByIdQuery<UpdateDocumentAssetCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .DocumentAssets
                .Include(a => a.DocumentAssetTags)
                .ThenInclude(a => a.Tag)
                .AsNoTracking()
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            var result = new UpdateDocumentAssetCommand()
            {
                Description = dbResult.Description,
                DocumentAssetId = dbResult.DocumentAssetId,
                Title = dbResult.Title
            };

            result.Tags = dbResult
                    .DocumentAssetTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t)
                    .ToArray();

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdateDocumentAssetCommand> query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
