using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.IO;

namespace Cofoundry.Domain
{
    public class GetImageAssetFileByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<ImageAssetFile>, ImageAssetFile>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<ImageAssetFile>, ImageAssetFile>
    {
        #region constructor

        private readonly IFileStoreService _fileStoreService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IImageAssetFileMapper _imageAssetFileMapper;

        public GetImageAssetFileByIdQueryHandler(
            IFileStoreService fileStoreService,
            IQueryExecutor queryExecutor,
            IImageAssetFileMapper imageAssetFileMapper
            )
        {
            _fileStoreService = fileStoreService;
            _queryExecutor = queryExecutor;
            _imageAssetFileMapper = imageAssetFileMapper;
        }

        #endregion

        #region execution

        public async Task<ImageAssetFile> ExecuteAsync(GetByIdQuery<ImageAssetFile> query, IExecutionContext executionContext)
        {
            // Render details is potentially cached so we query for this rather than direcvtly with the db
            var dbResult = await _queryExecutor.GetByIdAsync<ImageAssetRenderDetails>(query.Id);

            if (dbResult == null) return null;
            var fileName = Path.ChangeExtension(query.Id.ToString(), dbResult.Extension);
            var contentStream = await _fileStoreService.GetAsync(ImageAssetConstants.FileContainerName, fileName); ;

            if (contentStream == null)
            {
                throw new FileNotFoundException("Image asset file could not be found", fileName);
            }

            var result = _imageAssetFileMapper.Map(dbResult, contentStream);

            return result;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<ImageAssetFile> query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
