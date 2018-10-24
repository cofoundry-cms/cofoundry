using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Core.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Cofoundry.Web
{
    public class CofoundryAssetsController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IResizedImageAssetFileService _resizedImageAssetFileService;
        private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;
        private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;
        private readonly IMimeTypeService _mimeTypeService;
        private readonly ILogger<CofoundryAssetsController> _logger;
        private readonly ImageAssetsSettings _imageAssetsSettings;
        private readonly DocumentAssetsSettings _documentAssetsSettings;

        public CofoundryAssetsController(
            IQueryExecutor queryExecutor,
            IResizedImageAssetFileService resizedImageAssetFileService,
            IImageAssetRouteLibrary imageAssetRouteLibrary,
            IDocumentAssetRouteLibrary documentAssetRouteLibrary,
            IMimeTypeService mimeTypeService,
            ILogger<CofoundryAssetsController> logger,
            ImageAssetsSettings imageAssetsSettings,
            DocumentAssetsSettings documentAssetsSettings
            )
        {
            _queryExecutor = queryExecutor;
            _resizedImageAssetFileService = resizedImageAssetFileService;
            _imageAssetRouteLibrary = imageAssetRouteLibrary;
            _documentAssetRouteLibrary = documentAssetRouteLibrary;
            _mimeTypeService = mimeTypeService;
            _logger = logger;
            _imageAssetsSettings = imageAssetsSettings;
            _documentAssetsSettings = documentAssetsSettings;
        }

        #region images

        public async Task<ActionResult> Image(int imageAssetId, long fileStamp, string verificationToken, string fileName, string extension)
        {
            var settings = ImageResizeSettings.ParseFromQueryString(Request.Query);

            var getImageQuery = new GetImageAssetRenderDetailsByIdQuery(imageAssetId);
            var asset = await _queryExecutor.ExecuteAsync(getImageQuery);

            // additionally check that filestamp is not after the curent update date
            if (asset == null || !IsFileStampValid(fileStamp, asset.FileUpdateDate) || asset.VerificationToken != verificationToken)
            {
                return FileAssetNotFound("Image could not be found");
            }

            // if the title or filestamp is different, redirect to the correct url
            var sluggedFileName = SlugFormatter.ToSlug(asset.FileName);
            if (sluggedFileName != fileName || fileStamp.ToString() != asset.FileStamp)
            {
                var url = _imageAssetRouteLibrary.ImageAsset(asset, settings);
                return RedirectPermanent(url);
            }

            Stream stream = null;

            try
            {
                stream = await _resizedImageAssetFileService.GetAsync(asset, settings);
            }
            catch (FileNotFoundException ex)
            {
                // If the file exists but the file has gone missing, log and return a 404
                _logger.LogError(0, ex, "Image Asset exists, but has no file: {0}", imageAssetId);
                return FileAssetNotFound("File not found");
            }

            var contentType = _mimeTypeService.MapFromFileName("." + asset.FileExtension);
            SetCacheHeader(_imageAssetsSettings.CacheMaxAge);

            return new FileStreamResult(stream, contentType);
        }

        public async Task<ActionResult> Image_OldPath(int assetId, string fileName, string extension, int? cropSizeId)
        {
            var settings = ImageResizeSettings.ParseFromQueryString(Request.Query);

            var getImageQuery = new GetImageAssetRenderDetailsByIdQuery(assetId);
            var asset = await _queryExecutor.ExecuteAsync(getImageQuery);

            if (asset == null)
            {
                return FileAssetNotFound("Image could not be found");
            }

            var url = _imageAssetRouteLibrary.ImageAsset(asset, settings);
            return RedirectPermanent(url);
        }

        #endregion

        #region documents

        public async Task<ActionResult> Document(int documentAssetId, long fileStamp, string verificationToken, string fileName, string extension)
        {
            var file = await GetDocumentAssetFile(documentAssetId);

            var validationAction = ValidateDocumentFileRequest(fileStamp, verificationToken, fileName, file, false);
            if (validationAction != null) return validationAction;

            // Set the filename header separately to force "inline" content 
            // disposition even though a filename is specified.
            var contentDisposition = new ContentDispositionHeaderValue("inline");
            contentDisposition.SetHttpFileName(file.GetFileNameWithExtension());

            Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            SetCacheHeader(_documentAssetsSettings.CacheMaxAge);

            return File(file.ContentStream, file.ContentType);
        }

        public async Task<ActionResult> DocumentDownload(int documentAssetId, long fileStamp, string verificationToken, string fileName, string extension)
        {
            var file = await GetDocumentAssetFile(documentAssetId);

            var validationAction = ValidateDocumentFileRequest(fileStamp, verificationToken, fileName, file, true);
            if (validationAction != null) return validationAction;

            SetCacheHeader(_documentAssetsSettings.CacheMaxAge);

            return File(file.ContentStream, file.ContentType, file.GetFileNameWithExtension());
        }

        /// <summary>
        /// Old action for routing files without update timestamps and therefore
        /// not permanently cacheable.
        /// </summary>
        public async Task<ActionResult> File_OldPath(int assetId, string fileName, string extension)
        {
            var file = await GetDocumentAssetFile(assetId);

            if (file == null)
            {
                return FileAssetNotFound("File not found");
            }

            var url = _documentAssetRouteLibrary.DocumentAsset(file);
            return RedirectPermanent(url);
        }

        /// <summary>
        /// Old action for routing files without update timestamps and therefore
        /// not permanently cacheable.
        /// </summary>
        public async Task<ActionResult> FileDownload_OldPath(int assetId, string fileName, string extension)
        {
            var file = await GetDocumentAssetFile(assetId);

            if (file == null)
            {
                return FileAssetNotFound("File not found");
            }

            var url = _documentAssetRouteLibrary.DocumentAsset(file);
            return RedirectPermanent(url);
        }

        private async Task<DocumentAssetFile> GetDocumentAssetFile(int assetId)
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

        private ActionResult ValidateDocumentFileRequest(long fileStamp, string verificationToken, string fileName, DocumentAssetFile file, bool isDownload)
        {
            // additionally check that filestamp is not after the curent update date
            if (file == null || !IsFileStampValid(fileStamp, file.FileUpdateDate) || file.VerificationToken != verificationToken)
            {
                return FileAssetNotFound("Document file not found");
            }

            // if the title or filestamp is different, redirect to the correct url
            var sluggedFileName = SlugFormatter.ToSlug(file.FileName);
            if (sluggedFileName != fileName || fileStamp.ToString() != file.FileStamp)
            {
                var url = isDownload ? _documentAssetRouteLibrary.DocumentAssetDownload(file) : _documentAssetRouteLibrary.DocumentAsset(file);
                return RedirectPermanent(url);
            }

            return null;
        }

        #endregion

        private ContentResult FileAssetNotFound(string message)
        {
            ControllerContext.HttpContext.Response.StatusCode = 404;
            return new ContentResult() { Content = message };
        }

        private static bool IsFileStampValid(long fileStamp, DateTime fileUpdateDate)
        {
            if (fileStamp < 1) return false;

            var fileStampDate = AssetFileStampHelper.ToDate(fileStamp);

            return fileStampDate.HasValue && fileStampDate.Value.Ticks <= fileUpdateDate.Ticks;
        }

        private void SetCacheHeader(int maxAge)
        {
            Response.Headers[HeaderNames.CacheControl] = new[] { "public,max-age=" + maxAge };
        }
    }
}
