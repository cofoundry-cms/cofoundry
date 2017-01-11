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
    [AdminApiRoutePrefix("roles")]
    public class RolesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{roleId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public RolesApiController(
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
        public async Task<IHttpActionResult> Get([FromUri] SearchRolesQuery query)
        {
            if (query == null) query = new SearchRolesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int roleId)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(roleId));
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post(AddRoleCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Patch(int roleId, Delta<UpdateRoleCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, roleId, delta);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int roleId)
        {
            var command = new DeleteRoleCommand();
            command.RoleId = roleId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}