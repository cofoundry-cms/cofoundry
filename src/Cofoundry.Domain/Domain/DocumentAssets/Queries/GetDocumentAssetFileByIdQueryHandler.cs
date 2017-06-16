using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.IO;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetDocumentAssetFileByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<DocumentAssetFile>, DocumentAssetFile>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<DocumentAssetFile>, DocumentAssetFile>
    {
        private readonly IFileStoreService _fileStoreService;
        private readonly CofoundryDbContext _dbContext;

        public GetDocumentAssetFileByIdQueryHandler(
            IFileStoreService fileStoreService,
            CofoundryDbContext dbContext
            )
        {
            _fileStoreService = fileStoreService;
            _dbContext = dbContext;
        }

        public async Task<DocumentAssetFile> ExecuteAsync(GetByIdQuery<DocumentAssetFile> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .DocumentAssets
                .Where(f => f.DocumentAssetId == query.Id && !f.IsDeleted)
                .Select(f => new {
                    Extension = f.FileExtension,
                    ContentType = f.ContentType,
                    FileName = f.Title
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;
            var fileName = Path.ChangeExtension(query.Id.ToString(), dbResult.Extension);

            var result = new DocumentAssetFile();
            result.DocumentAssetId = query.Id;
            result.ContentType = dbResult.ContentType;
            result.ContentStream = _fileStoreService.Get(DocumentAssetConstants.FileContainerName, fileName);
            result.FileName = FilePathHelper.CleanFileName(Path.ChangeExtension(dbResult.FileName, dbResult.Extension), fileName);

            if (result.ContentStream == null)
            {
                throw new FileNotFoundException("DocumentAsset file could not be found", fileName);
            }

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<DocumentAssetFile> query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
