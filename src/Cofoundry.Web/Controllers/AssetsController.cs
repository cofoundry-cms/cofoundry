using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core.ErrorLogging;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    public class AssetsController : Controller
    {
        #region Constructors

        private readonly IQueryExecutor _queryExecutor;
        private readonly IResizedImageAssetFileService _resizedImageAssetFileService;
        private readonly IErrorLoggingService _errorLoggingService;
        private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;

        public AssetsController(
            IQueryExecutor queryExecutor,
            IResizedImageAssetFileService resizedImageAssetFileService,
            IErrorLoggingService errorLoggingService,
            IImageAssetRouteLibrary imageAssetRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _resizedImageAssetFileService = resizedImageAssetFileService;
            _errorLoggingService = errorLoggingService;
            _imageAssetRouteLibrary = imageAssetRouteLibrary;
        }

        #endregion

        private ContentResult FileAssetNotFound(string message)
        {
            ControllerContext.HttpContext.Response.StatusCode = 404;
            return new ContentResult() { Content = message };
        }

        //[OutputCache(Duration = 60 * 60 * 24 * 30, Location = OutputCacheLocation.Downstream)]
        public ActionResult Image(int assetId, string fileName, string extension, int? cropSizeId)
        {
            var qs = Request.Url.Query;
            var settings = ImageResizeSettings.ParseFromQueryString(qs);

            var asset = _queryExecutor.GetById<ImageAssetRenderDetails>(assetId);
            if (asset == null)
            {
                return FileAssetNotFound("Image could not be found");
            }

            if (SlugFormatter.ToSlug(asset.FileName) != fileName)
            {
                var url = _imageAssetRouteLibrary.ImageAsset(asset, settings);
                return RedirectPermanent(url);
            }

            DateTime lastModified = DateTime.SpecifyKind(asset.UpdateDate, DateTimeKind.Utc);
            // Round the ticks down (see http://stackoverflow.com/a/1005222/486434), because http headers are only accurate to seconds, so get rounded down
            lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerSecond));

            if (!string.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
            {
                DateTime ifModifiedSince;
                if (DateTime.TryParse(Request.Headers["If-Modified-Since"], out ifModifiedSince) && lastModified <= ifModifiedSince.ToUniversalTime())
                {
                    return new HttpStatusCodeResult(304, "Not Modified");
                }
            }

            Stream stream = null;

            try
            {
                stream = _resizedImageAssetFileService.Get(asset, settings);
            }
            catch (FileNotFoundException ex)
            {
                // If the file exists but the file has gone missing, log and return a 404
                _errorLoggingService.Log(ex);
                return FileAssetNotFound("File not found");
            }

            // Expire the image, so browsers always check with the server, but also send a last modified date so we can check for If-Modified-Since on the next request and return a 304 Not Modified.
            Response.Cache.SetExpires(DateTime.UtcNow.AddMonths(-1));
            Response.Cache.SetLastModified(lastModified);
            
            var contentType = MimeMapping.GetMimeMapping(asset.Extension);
            return new FileStreamResult(stream, contentType);
        }
        
        [OutputCache(Duration = 60 * 60, Location = OutputCacheLocation.Client)]
        public ActionResult File(int assetId, string fileName, string extension)
        {
            DocumentAssetFile file = null;

            try
            {
                file = _queryExecutor.GetById<DocumentAssetFile>(assetId);
            }
            catch (FileNotFoundException ex)
            {
                // If the file exists but the file has gone missing, log and return a 404
                _errorLoggingService.Log(ex);
            }

            if (file == null)
            {
                return FileAssetNotFound("File not found");
            }

            return File(file.ContentStream, file.ContentType, file.FileName);
        }
    }
}
