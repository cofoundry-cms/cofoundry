using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("images")]
    public class ImagesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{imageAssetId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public ImagesApiController(
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
        public async Task<IHttpActionResult> Get([FromUri] SearchImageAssetSummariesQuery query, [FromUri] GetByIdRangeQuery<ImageAssetRenderDetails> rangeQuery)
        {
            if (rangeQuery != null && rangeQuery.Ids != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.ToFilteredAndOrderedCollection(rangeQuery.Ids));
            }

            if (query == null) query = new SearchImageAssetSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int imageAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<ImageAssetDetails>(imageAssetId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post(AddImageAssetCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Patch(int imageAssetId, Delta<UpdateImageAssetCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, imageAssetId, delta);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int imageAssetId)
        {
            var command = new DeleteImageAssetCommand();
            command.ImageAssetId = imageAssetId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }


        #endregion

        #endregion
    }
}