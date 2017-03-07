using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiAuthorize]
    [ValidateApiAntiForgeryToken]
    [RoutePrefix(RouteConstants.ApiRoutePrefix + "/Account")]
    public class AccountApiController : ApiController
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
        [Route]
        public async Task<IHttpActionResult> Get()
        {
            var query = new GetCurrentUserAccountDetailsQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        #endregion

        #region commands

        [Route]
        [HttpPatch]
        public async Task<IHttpActionResult> Patch(Delta<UpdateCurrentUserAccountCommand> delta)
        {
            var userId = _userContextService.GetCurrentContext().UserId.Value;

            return await _apiResponseHelper.RunCommandAsync(this, userId, delta);
        }

        [HttpPut]
        [Route("Password")]
        public async Task<IHttpActionResult> PutPassword(UpdateCurrentUserUserPasswordCommandDto dto)
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