using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{

    /// <summary>
    /// Service for resizing and caching the resulting image.
    /// </summary>
    public class SimpleResizedImageAssetFileService : IResizedImageAssetFileService
    {
        #region private member variables

        private readonly IFileStoreService _fileService;
        private readonly IQueryExecutor _queryExecutor;

        #endregion

        #region constructor

        public SimpleResizedImageAssetFileService(
            IFileStoreService fileService,
            IQueryExecutor queryExecutor
            )
        {
            _fileService = fileService;
            _queryExecutor = queryExecutor;
        }

        #endregion

        public Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings inputSettings)
        {
            // Resizing only supported via plugin
            return GetFileStreamAsync(asset.ImageAssetId);
        }

        public void Clear(int imageAssetId)
        {
            // nothing to clear
        }

        #region private methods

        private async Task<Stream> GetFileStreamAsync(int imageAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<ImageAssetFile>(imageAssetId);

            if (result == null || result.ContentStream == null)
            {
                throw new FileNotFoundException(imageAssetId.ToString());
            }

            return result.ContentStream;
        }

        #endregion
    }
}
