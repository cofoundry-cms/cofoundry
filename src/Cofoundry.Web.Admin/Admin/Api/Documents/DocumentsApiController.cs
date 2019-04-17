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
    public class DocumentsApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IFormFileUploadedFileFactory _formFileUploadedFileFactory;

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

        #region queries

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

        public async Task<IActionResult> GetById(int documentAssetId)
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

        [HttpPut]
        public async Task<IActionResult> Put(int documentAssetId, UpdateDocumentAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int documentAssetId)
        {
            var command = new DeleteDocumentAssetCommand();
            command.DocumentAssetId = documentAssetId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }


        #endregion
    }
}