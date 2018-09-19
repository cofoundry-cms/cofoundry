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
        #region constructor

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

        #endregion

        #region routes

        #region queries

        public async Task<IActionResult> Get()
        {
            var query = new GetCurrentUserAccountDetailsQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        #endregion

        #region commands

        public async Task<IActionResult> Patch([FromBody] IDelta<UpdateCurrentUserAccountCommand> delta)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            var userId = userContext.UserId.Value;

            return await _apiResponseHelper.RunCommandAsync(this, userId, delta);
        }

        public async Task<IActionResult> PutPassword([FromBody] UpdateCurrentUserPasswordCommandDto dto)
        {
            var command = new UpdateCurrentUserPasswordCommand()
            {
                OldPassword = dto.OldPassword,
                NewPassword = dto.NewPassword
            };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}