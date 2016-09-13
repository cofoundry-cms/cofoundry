using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.ErrorLogging;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper service for resolving an asset urls
    /// </summary>
    public class ImageAssetUrlResolutionService : IImageAssetUrlResolutionService
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly IResizedImageAssetFileService _resizedImageAssetFileService;
        private readonly IErrorLoggingService _errorLoggingService;

        public ImageAssetUrlResolutionService(
            IQueryExecutor queryExecutor,
            IResizedImageAssetFileService resizedImageAssetFileService,
            IErrorLoggingService errorLoggingService
            )
        {
            _queryExecutor = queryExecutor;
            _resizedImageAssetFileService = resizedImageAssetFileService;
            _errorLoggingService = errorLoggingService;
        }

        #endregion

        #region public methods

        public string GetUrl(int? id, IImageResizeSettings settings = null)
        {
            if (!id.HasValue) return string.Empty;

            var asset = _queryExecutor.GetById<ImageAssetRenderDetails>(id.Value);

            return AssetRouteLibrary.ImageAsset(asset, settings);
        }

        public string GetDataUrl(int? id, IImageResizeSettings settings = null)
        {
            if (!id.HasValue) return string.Empty;

            var asset = _queryExecutor.GetById<ImageAssetRenderDetails>(id.Value);

            return GetDataUrl(asset, settings);
        }

        public string GetDataUrl(IImageAssetRenderable asset, IImageResizeSettings settings = null)
        {
            if (asset == null) return string.Empty;

            Stream stream = null;

            try
            {
                stream = _resizedImageAssetFileService.Get(asset, settings);
            }
            catch (FileNotFoundException ex)
            {
                _errorLoggingService.Log(ex);
                return string.Empty;
            }

            byte[] EncodeBuffer;
            using (stream)
            {
                int Length = Convert.ToInt32(stream.Length);
                EncodeBuffer = new byte[Length];

                stream.Read(EncodeBuffer, 0, Length);
            }

            var contentType = MimeMapping.GetMimeMapping(asset.Extension);
            return string.Format("data:{0};base64,{1}", contentType, Convert.ToBase64String(EncodeBuffer));
        }

        #endregion
    }
}
