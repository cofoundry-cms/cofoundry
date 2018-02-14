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
        : IAsyncQueryHandler<GetDocumentAssetFileByIdQuery, DocumentAssetFile>
        , IPermissionRestrictedQueryHandler<GetDocumentAssetFileByIdQuery, DocumentAssetFile>
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

        public async Task<DocumentAssetFile> ExecuteAsync(GetDocumentAssetFileByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .DocumentAssets
                .Where(f => f.DocumentAssetId == query.DocumentAssetId && !f.IsDeleted)
                .Select(f => new {
                    Extension = f.FileExtension,
                    ContentType = f.ContentType,
                    FileName = f.Title
                })
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;
            var fileName = Path.ChangeExtension(query.DocumentAssetId.ToString(), dbResult.Extension);

            var result = new DocumentAssetFile();
            result.DocumentAssetId = query.DocumentAssetId;
            result.ContentType = dbResult.ContentType;
            result.ContentStream = await _fileStoreService.GetAsync(DocumentAssetConstants.FileContainerName, fileName);
            result.FileName = FilePathHelper.CleanFileName(Path.ChangeExtension(dbResult.FileName, dbResult.Extension), fileName);

            if (result.ContentStream == null)
            {
                throw new FileNotFoundException("DocumentAsset file could not be found", fileName);
            }

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetFileByIdQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
