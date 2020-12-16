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
    public class GetAllDocumentAssetFileTypesQueryHandler 
        : IQueryHandler<GetAllDocumentAssetFileTypesQuery, ICollection<DocumentAssetFileType>>
        , IPermissionRestrictedQueryHandler<GetAllDocumentAssetFileTypesQuery, ICollection<DocumentAssetFileType>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetAllDocumentAssetFileTypesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<ICollection<DocumentAssetFileType>> ExecuteAsync(GetAllDocumentAssetFileTypesQuery query, IExecutionContext executionContext)
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllDocumentAssetFileTypesQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
