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
    [AdminApiRoutePrefix("users")]
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
        [Route]
        public async Task<IHttpActionResult> Get([FromUri] SearchUserSummariesQuery query)
        {
            if (query == null) query = new SearchUserSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int userId)
        {
            var result = await _queryExecutor.GetByIdAsync<UserDetails>(userId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post(AddUserCommand command)
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

        [HttpPatch]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Patch(int userId, Delta<UpdateUserCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, userId, delta);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}