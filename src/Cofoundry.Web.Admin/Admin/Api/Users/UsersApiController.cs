using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("users")]
    public class UsersApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{userId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public UsersApiController(
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
        public async Task<IActionResult> Get([FromQuery] SearchUserSummariesQuery query)
        {
            if (query == null) query = new SearchUserSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        [HttpGet(ID_ROUTE)]
        public async Task<IActionResult> Get(int userId)
        {
            var query = new GetUserDetailsByIdQuery(userId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUserCommand command)
        {
            if (command.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                // TODO: We have a separate command here for adding Cofoundry Admin users, but we could re-use the same one
                // and separate the notification part out of the handler and make it a separate function in the admin panel.
                var userCommand = new AddCofoundryUserCommand();
                userCommand.Email = command.Email;
                userCommand.FirstName = command.FirstName;
                userCommand.LastName = command.LastName;
                userCommand.RoleId = command.RoleId;

                return await _apiResponseHelper.RunCommandAsync(this, userCommand);
            }
            else
            {
                return await _apiResponseHelper.RunCommandAsync(this, command);
            }
        }

        [HttpPatch(ID_ROUTE)]
        public async Task<IActionResult> Patch(int userId, [FromBody] IDelta<UpdateUserCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, userId, delta);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}