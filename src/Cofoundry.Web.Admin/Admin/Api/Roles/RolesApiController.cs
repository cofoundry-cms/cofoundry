using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        public async Task<JsonResult> Get([FromQuery] SearchRolesQuery query)
        {
            if (query == null) query = new SearchRolesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetById(int roleId)
        {
            var query = new GetRoleDetailsByIdQuery(roleId);
            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public Task<JsonResult> Post([FromBody] AddRoleCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Patch(int roleId, [FromBody] IDelta<UpdateRoleCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(roleId, delta);
        }

        public Task<JsonResult> Delete(int roleId)
        {
            var command = new DeleteRoleCommand();
            command.RoleId = roleId;

            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}