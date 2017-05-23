using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    [Route(RouteConstants.ApiRoutePrefix + "/account")]
    public class AccountApiController : BaseAdminApiController
    {
        #region private member variables

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IUserContextService _userContextService;

        #endregion

        #region constructor

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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = new GetCurrentUserAccountDetailsQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        #endregion

        #region commands

        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] Delta<UpdateCurrentUserAccountCommand> delta)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            var userId = userContext.UserId.Value;

            return await _apiResponseHelper.RunCommandAsync(this, userId, delta);
        }

        [HttpPut("Password")]
        public async Task<IActionResult> PutPassword([FromBody] UpdateCurrentUserUserPasswordCommandDto dto)
        {
            var command = new UpdateCurrentUserUserPasswordCommand()
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