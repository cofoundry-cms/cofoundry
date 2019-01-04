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

        public ImagesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            IFormFileUploadedFileFactory formFileUploadedFileFactory
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _formFileUploadedFileFactory = formFileUploadedFileFactory;
        }

        #region queries

        public async Task<IActionResult> Get(
            [FromQuery] SearchImageAssetSummariesQuery query, 
            [FromQuery] GetImageAssetRenderDetailsByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.ImageAssetIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.FilterAndOrderByKeys(rangeQuery.ImageAssetIds));
            }

            if (query == null) query = new SearchImageAssetSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        public async Task<IActionResult> GetById(int imageAssetId)
        {
            var query = new GetImageAssetDetailsByIdQuery(imageAssetId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post(AddImageAssetCommand command, IFormFile file) 
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int imageAssetId, UpdateImageAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int imageAssetId)
        {
            var command = new DeleteImageAssetCommand();
            command.ImageAssetId = imageAssetId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}