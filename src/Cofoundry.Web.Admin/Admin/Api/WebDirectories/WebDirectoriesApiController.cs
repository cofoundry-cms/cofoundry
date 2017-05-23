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
    [Route(RouteConstants.ApiRoutePrefix + "/WebDirectories")]
    public class WebDirectoriesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{webDirectoryId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public WebDirectoriesApiController(
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
        public async Task<IActionResult> Get()
        {
            var results = await _queryExecutor.GetAllAsync<WebDirectoryRoute>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var query = new GetWebDirectoryTreeQuery();
            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int webDirectoryId)
        {
            var result = await _queryExecutor.GetByIdAsync<WebDirectoryNode>(webDirectoryId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddWebDirectoryCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch(ID_ROUTE)]
        public async Task<IActionResult> Patch(int webDirectoryId, [FromBody] Delta<UpdateWebDirectoryCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, webDirectoryId, delta);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int webDirectoryId)
        {
            var command = new DeleteWebDirectoryCommand();
            command.WebDirectoryId = webDirectoryId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}