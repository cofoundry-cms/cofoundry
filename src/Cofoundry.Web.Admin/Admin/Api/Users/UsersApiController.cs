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

        public async Task<JsonResult> Get([FromQuery] SearchUserSummariesQuery query)
        {
            if (query == null) query = new SearchUserSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }
        
        public async Task<JsonResult> GetById(int userId)
        {
            var query = new GetUserDetailsByIdQuery(userId);
            var result = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(result);
        }

        #endregion

        #region commands

        public async Task<JsonResult> Post([FromBody] AddUserCommand command)
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

                return await _apiResponseHelper.RunCommandAsync(userCommand);
            }
            else
            {
                return await _apiResponseHelper.RunCommandAsync(command);
            }
        }

        public Task<JsonResult> Patch(int userId, [FromBody] IDelta<UpdateUserCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(userId, delta);
        }

        public Task<JsonResult> Delete(int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        #endregion
    }
}