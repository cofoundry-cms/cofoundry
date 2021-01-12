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

        public async Task<JsonResult> Get(
            [FromQuery] SearchDocumentAssetSummariesQuery query, 
            [FromQuery] GetDocumentAssetRenderDetailsByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.DocumentAssetIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(rangeResults.FilterAndOrderByKeys(rangeQuery.DocumentAssetIds));
            }

            if (query == null) query = new SearchDocumentAssetSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(int documentAssetId)
        {
            var query = new GetDocumentAssetDetailsByIdQuery(documentAssetId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(result);
        }

        #endregion

        #region commands

        public Task<JsonResult> Post(AddDocumentAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Put(int documentAssetId, UpdateDocumentAssetCommand command, IFormFile file)
        {
            command.File = _formFileUploadedFileFactory.Create(file);

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int documentAssetId)
        {
            var command = new DeleteDocumentAssetCommand();
            command.DocumentAssetId = documentAssetId;

            return _apiResponseHelper.RunCommandAsync(command);
        }


        #endregion
    }
}