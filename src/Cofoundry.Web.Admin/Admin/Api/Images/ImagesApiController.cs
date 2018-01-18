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
    [AdminApiRoute("images")]
    public class ImagesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{imageAssetId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IFormFileUploadedFileFactory _formFileUploadedFileFactory;

        #endregion

        #region constructor

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

        #endregion

        #region routes

        #region queries

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SearchImageAssetSummariesQuery query, [FromQuery] GetByIdRangeQuery<ImageAssetRenderDetails> rangeQuery)
        {
            if (rangeQuery != null && rangeQuery.Ids != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.FilterAndOrderByKeys(rangeQuery.Ids));
            }

            if (query == null) query = new SearchImageAssetSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int imageAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<ImageAssetDetails>(imageAssetId);
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

        [HttpPut(ID_ROUTE)]
        public async Task<IActionResult> Put(int imageAssetId, UpdateImageAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int imageAssetId)
        {
            var command = new DeleteImageAssetCommand();
            command.ImageAssetId = imageAssetId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}