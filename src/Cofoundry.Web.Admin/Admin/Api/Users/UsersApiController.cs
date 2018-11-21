using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class UsersApiController : BaseAdminApiController
    {
        private const string ID_ROUTE = "{userId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public UsersApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<IActionResult> Get([FromQuery] SearchUserSummariesQuery query)
        {
            if (query == null) query = new SearchUserSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        public async Task<IActionResult> GetById(int userId)
        {
            var query = new GetUserDetailsByIdQuery(userId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        public async Task<IActionResult> Post([FromBody] AddUserWithTemporaryPasswordCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        public async Task<IActionResult> Patch(int userId, [FromBody] IDelta<UpdateUserCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, userId, delta);
        }

        public async Task<IActionResult> Delete(int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}