using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class AccountApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IUserContextService _userContextService;

        public AccountApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            IUserContextService userContextService
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _userContextService = userContextService;
        }

        public async Task<JsonResult> Get()
        {
            var query = new GetCurrentUserDetailsQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }
        
        public async Task<JsonResult> Patch([FromBody] IDelta<UpdateCurrentUserCommand> delta)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            var userId = userContext.UserId.Value;

            return await _apiResponseHelper.RunCommandAsync(userId, delta);
        }

        public Task<JsonResult> PutPassword([FromBody] UpdateCurrentUserPasswordCommandDto dto)
        {
            var command = new UpdateCurrentUserPasswordCommand()
            {
                OldPassword = dto.OldPassword,
                NewPassword = dto.NewPassword
            };

            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}