using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("pages")]
    public class PagesApiController : BaseAdminApiController
    {
        private const string ID_ROUTE = "{pageId:int}";

        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public PagesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region queries

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] SearchPageSummariesQuery query,
            [FromQuery] GetPageSummariesByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.PageIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.FilterAndOrderByKeys(rangeQuery.PageIds));
            }

            if (query == null) query = new SearchPageSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int pageId)
        {
            var query = new GetPageDetailsByIdQuery(pageId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }
        

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddPageCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch(ID_ROUTE)]
        public async Task<IActionResult> Patch(int pageId, [FromBody] IDelta<UpdatePageCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, pageId, delta);
        }

        [HttpPut(ID_ROUTE + "/url")]
        public async Task<IActionResult> PutPageUrl(int pageId, [FromBody] UpdatePageUrlCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int pageId)
        {
            var command = new DeletePageCommand();
            command.PageId = pageId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPost(ID_ROUTE + "/duplicate")]
        public async Task<IActionResult> Post([FromBody] DuplicatePageCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}