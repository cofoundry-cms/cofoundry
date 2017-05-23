using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("images")]
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
        public async Task<IActionResult> Get([FromQuery] SearchImageAssetSummariesQuery query, [FromQuery] GetByIdRangeQuery<ImageAssetRenderDetails> rangeQuery)
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

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int imageAssetId)
        {
            var result = await _queryExecutor.GetByIdAsync<ImageAssetDetails>(imageAssetId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddImageAssetCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch(ID_ROUTE)]
        public async Task<IActionResult> Patch(int imageAssetId, [FromBody] Delta<UpdateImageAssetCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, imageAssetId, delta);
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