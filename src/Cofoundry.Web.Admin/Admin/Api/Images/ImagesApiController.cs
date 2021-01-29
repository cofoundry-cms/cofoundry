using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web.Admin
{
    public class ImagesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IFormFileUploadedFileFactory _formFileUploadedFileFactory;
        private readonly ImageAssetsSettings _imageAssetsSettings;

        public ImagesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            IFormFileUploadedFileFactory formFileUploadedFileFactory,
            ImageAssetsSettings imageAssetsSettings
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _formFileUploadedFileFactory = formFileUploadedFileFactory;
            _imageAssetsSettings = imageAssetsSettings;
        }

        #region queries

        public async Task<JsonResult> Get(
            [FromQuery] SearchImageAssetSummariesQuery query, 
            [FromQuery] GetImageAssetRenderDetailsByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.ImageAssetIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(rangeResults.FilterAndOrderByKeys(rangeQuery.ImageAssetIds));
            }

            if (query == null) query = new SearchImageAssetSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(int imageAssetId)
        {
            var query = new GetImageAssetDetailsByIdQuery(imageAssetId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(result);
        }

        public JsonResult GetSettings()
        {
            return _apiResponseHelper.SimpleQueryResponse(new { 
                _imageAssetsSettings.MaxUploadWidth,
                _imageAssetsSettings.MaxUploadHeight
            });
        }

        #endregion

        #region commands

        public Task<JsonResult> Post(AddImageAssetCommand command, IFormFile file) 
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Put(int imageAssetId, UpdateImageAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int imageAssetId)
        {
            var command = new DeleteImageAssetCommand();
            command.ImageAssetId = imageAssetId;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        #endregion
    }
}