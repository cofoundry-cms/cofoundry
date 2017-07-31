using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("roles")]
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
        public async Task<IActionResult> Get([FromQuery] SearchRolesQuery query)
        {
            if (query == null) query = new SearchRolesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int roleId)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(roleId));
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddRoleCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch(ID_ROUTE)]
        public async Task<IActionResult> Patch(int roleId, [FromBody] IDelta<UpdateRoleCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, roleId, delta);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int roleId)
        {
            var command = new DeleteRoleCommand();
            command.RoleId = roleId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}