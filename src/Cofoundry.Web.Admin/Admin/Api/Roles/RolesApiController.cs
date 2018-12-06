using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class RolesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public RolesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<IActionResult> Get([FromQuery] SearchRolesQuery query)
        {
            if (query == null) query = new SearchRolesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        public async Task<IActionResult> GetById(int roleId)
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

        [HttpPatch]
        public async Task<IActionResult> Patch(int roleId, [FromBody] IDelta<UpdateRoleCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, roleId, delta);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int roleId)
        {
            var command = new DeleteRoleCommand();
            command.RoleId = roleId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}