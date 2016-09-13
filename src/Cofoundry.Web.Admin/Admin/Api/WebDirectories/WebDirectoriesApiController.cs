using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [RoutePrefix(RouteConstants.ApiRoutePrefix + "/WebDirectories")]
    public class WebDirectoriesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{webDirectoryId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public WebDirectoriesApiController(
            IQueryExecutor queryExecutor,
            ApiResponseHelper apiResponseHelper
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
        public async Task<IHttpActionResult> Get()
        {
            var results = await _queryExecutor.GetAllAsync<WebDirectoryRoute>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route("tree")]
        public async Task<IHttpActionResult> GetTree()
        {
            var query = new GetWebDirectoryTreeQuery();
            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int webDirectoryId)
        {
            var result = await _queryExecutor.GetByIdAsync<WebDirectoryNode>(webDirectoryId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post(AddWebDirectoryCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Patch(int webDirectoryId, Delta<UpdateWebDirectoryCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, webDirectoryId, delta);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int webDirectoryId)
        {
            var command = new DeleteWebDirectoryCommand();
            command.WebDirectoryId = webDirectoryId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}