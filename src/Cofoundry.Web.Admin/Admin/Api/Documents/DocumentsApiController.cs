using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("documents")]
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
        [Route]
        public async Task<IHttpActionResult> Get([FromUri] SearchDocumentAssetSummariesQuery query)
        {
            if (query == null) query = new SearchDocumentAssetSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int documentAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<DocumentAssetDetails>(documentAssetId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post(AddDocumentAssetCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Patch(int documentAssetId, Delta<UpdateDocumentAssetCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, documentAssetId, delta);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int documentAssetId)
        {
            var command = new DeleteDocumentAssetCommand();
            command.DocumentAssetId = documentAssetId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }


        #endregion

        #endregion
    }
}