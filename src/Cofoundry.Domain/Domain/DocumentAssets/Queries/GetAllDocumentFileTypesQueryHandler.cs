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
    public class GetAllDocumentFileTypesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<DocumentAssetFileType>, IEnumerable<DocumentAssetFileType>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<DocumentAssetFileType>, IEnumerable<DocumentAssetFileType>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetAllDocumentFileTypesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<IEnumerable<DocumentAssetFileType>> ExecuteAsync(GetAllQuery<DocumentAssetFileType> query, IExecutionContext executionContext)
        {
            var result = await _dbContext
                .DocumentAssets
                .AsNoTracking()
                .Select(a => a.FileExtension)
                .Distinct()
                .OrderBy(a => a)
                .Select(e => new DocumentAssetFileType() { FileExtension = e })
                .ToListAsync();

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<DocumentAssetFileType> query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
