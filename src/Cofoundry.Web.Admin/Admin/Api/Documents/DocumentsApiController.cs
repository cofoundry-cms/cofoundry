using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("documents")]
    public class DocumentsApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{documentAssetId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public DocumentsApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region queries

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SearchDocumentAssetSummariesQuery query)
        {
            if (query == null) query = new SearchDocumentAssetSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int documentAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<DocumentAssetDetails>(documentAssetId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddDocumentAssetCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch(ID_ROUTE)]
        public async Task<IActionResult> Patch(int documentAssetId, [FromBody] Delta<UpdateDocumentAssetCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, documentAssetId, delta);
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