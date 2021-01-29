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
    public class PagesApiController : BaseAdminApiController
    {
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

        #region queries

        public async Task<JsonResult> Get(
            [FromQuery] SearchPageSummariesQuery query,
            [FromQuery] GetPageSummariesByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.PageIds != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(rangeResults.FilterAndOrderByKeys(rangeQuery.PageIds));
            }

            if (query == null) query = new SearchPageSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(int pageId)
        {
            var query = new GetPageDetailsByIdQuery(pageId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(result);
        }
        

        #endregion

        #region commands

        public Task<JsonResult> Post([FromBody] AddPageCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Patch(int pageId, [FromBody] IDelta<UpdatePageCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(pageId, delta);
        }

        public Task<JsonResult> PutPageUrl(int pageId, [FromBody] UpdatePageUrlCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int pageId)
        {
            var command = new DeletePageCommand();
            command.PageId = pageId;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public async Task<JsonResult> PostDuplicate([FromBody] DuplicatePageCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(command);
        }

        #endregion
    }
}