using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Cofoundry.Web
{
    public class CofoundryAssetsController : Controller
    {
        #region Constructors

        private readonly IQueryExecutor _queryExecutor;
        private readonly IResizedImageAssetFileService _resizedImageAssetFileService;
        private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;
        private readonly IMimeTypeService _mimeTypeService;
        private readonly ILogger<CofoundryAssetsController> _logger;

        public CofoundryAssetsController(
            IQueryExecutor queryExecutor,
            IResizedImageAssetFileService resizedImageAssetFileService,
            IImageAssetRouteLibrary imageAssetRouteLibrary,
            IMimeTypeService mimeTypeService,
            ILogger<CofoundryAssetsController> logger
            )
        {
            _queryExecutor = queryExecutor;
            _resizedImageAssetFileService = resizedImageAssetFileService;
            _imageAssetRouteLibrary = imageAssetRouteLibrary;
            _mimeTypeService = mimeTypeService;
            _logger = logger;
        }

        #endregion

        private ContentResult FileAssetNotFound(string message)
        {
            ControllerContext.HttpContext.Response.StatusCode = 404;
            return new ContentResult() { Content = message };
        }

        //[OutputCache(Duration = 60 * 60 * 24 * 30, Location = OutputCacheLocation.Downstream)]
        public async Task<ActionResult> Image(int assetId, string fileName, string extension, int? cropSizeId)
        {
            var settings = ImageResizeSettings.ParseFromQueryString(Request.Query);

            var getImageQuery = new GetImageAssetRenderDetailsByIdQuery(assetId);
            var asset = await _queryExecutor.ExecuteAsync(getImageQuery);

            if (asset == null)
            {
                return FileAssetNotFound("Image could not be found");
            }

            if (SlugFormatter.ToSlug(asset.FileName) != fileName)
            {
                var url = _imageAssetRouteLibrary.ImageAsset(asset, settings);
                return RedirectPermanent(url);
            }

            var lastModified = DateTime.SpecifyKind(asset.UpdateDate, DateTimeKind.Utc);
            // Round the ticks down (see http://stackoverflow.com/a/1005222/486434), because http headers are only accurate to seconds, so get rounded down
            lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerSecond));

            if (!string.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
            {
                DateTime ifModifiedSince;
                if (DateTime.TryParse(Request.Headers["If-Modified-Since"], out ifModifiedSince) && lastModified <= ifModifiedSince.ToUniversalTime())
                {
                    return StatusCode(304);
                }
            }

            Stream stream = null;

            try
            {
                stream = await _resizedImageAssetFileService.GetAsync(asset, settings);
            }
            catch (FileNotFoundException ex)
            {
                // If the file exists but the file has gone missing, log and return a 404
                _logger.LogError(0, ex, "Image Asset exists, but has no file: {0}", assetId);
                return FileAssetNotFound("File not found");
            }

            // Expire the image, so browsers always check with the server, but also send a last modified date so we can check for If-Modified-Since on the next request and return a 304 Not Modified.
            var headers = Response.GetTypedHeaders();
            headers.Expires = DateTime.UtcNow.AddMonths(-1);
            headers.LastModified = lastModified;
            
            var contentType = _mimeTypeService.MapFromFileName("." + asset.Extension);
            return new FileStreamResult(stream, contentType);
        }
        
        [ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult> File(int assetId, string fileName, string extension)
        {
            var file = await GetFile(assetId);

            if (file == null)
            {
                return FileAssetNotFound("File not found");
            }

            // Set the filename header separately to force "inline" content 
            // disposition even though a filename is specified.
            var contentDisposition = new ContentDispositionHeaderValue("inline");
            contentDisposition.SetHttpFileName(file.FileName);
            Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();

            return File(file.ContentStream, file.ContentType);
        }

        [ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult> FileDownload(int assetId, string fileName, string extension)
        {
            var file = await GetFile(assetId);

            if (file == null)
            {
                return FileAssetNotFound("File not found");
            }

            return File(file.ContentStream, file.ContentType, file.FileName);
        }

        private async Task<DocumentAssetFile> GetFile(int assetId)
        {
            DocumentAssetFile file = null;

            try
            {
                var query = new GetDocumentAssetFileByIdQuery(assetId);
                file = await _queryExecutor.ExecuteAsync(query);
            }
            catch (FileNotFoundException ex)
            {
                // If the file exists but the file has gone missing, log and return a 404
                _logger.LogError(0, ex, "Document Asset exists, but has no file: {0}", assetId);
            }

            return file;
        }
    }
}
