using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Http;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("documents")]
    public class DocumentsApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{documentAssetId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IFormFileUploadedFileFactory _formFileUploadedFileFactory;

        #endregion

        #region constructor

        public DocumentsApiController(
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
        public async Task<IActionResult> Get(
            [FromQuery] SearchDocumentAssetSummariesQuery query, 
            [FromQuery] GetDocumentAssetRenderDetailsByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.DocumentAssetIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.FilterAndOrderByKeys(rangeQuery.DocumentAssetIds));
            }

            if (query == null) query = new SearchDocumentAssetSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int documentAssetId)
        {
            var query = new GetDocumentAssetDetailsByIdQuery(documentAssetId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post(AddDocumentAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE)]
        public async Task<IActionResult> Put(int documentAssetId, UpdateDocumentAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int documentAssetId)
        {
            var command = new DeleteDocumentAssetCommand();
            command.DocumentAssetId = documentAssetId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }


        #endregion

        #endregion
    }
}