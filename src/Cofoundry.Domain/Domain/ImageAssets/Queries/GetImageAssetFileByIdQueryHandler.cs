using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.IO;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetImageAssetFileByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<ImageAssetFile>, ImageAssetFile>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<ImageAssetFile>, ImageAssetFile>
    {
        #region constructor

        private readonly IFileStoreService _fileStoreService;
        private readonly IQueryExecutor _queryExecutor;

        public GetImageAssetFileByIdQueryHandler(
            IFileStoreService fileStoreService,
            IQueryExecutor queryExecutor
            )
        {
            _fileStoreService = fileStoreService;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task<ImageAssetFile> ExecuteAsync(GetByIdQuery<ImageAssetFile> query, IExecutionContext executionContext)
        {
            var dbResult = await _queryExecutor.GetByIdAsync<ImageAssetRenderDetails>(query.Id);

            if (dbResult == null) return null;
            var fileName = Path.ChangeExtension(query.Id.ToString(), dbResult.Extension);

            var result = Mapper.Map<ImageAssetFile>(dbResult);
            result.ContentStream = _fileStoreService.Get(ImageAssetConstants.FileContainerName, fileName);

            if (result.ContentStream == null)
            {
                throw new FileNotFoundException("Image asset file could not be found", fileName);
            }

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
