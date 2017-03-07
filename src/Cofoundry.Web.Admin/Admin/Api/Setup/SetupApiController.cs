using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cofoundry.Domain;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("setup")]
    [ValidateApiAntiForgeryToken]
    public class SetupApiController : ApiController
    {
        #region private member variables

        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public SetupApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region commands

        [HttpPost]
        [Route]
        public async Task<IHttpActionResult> Post(SetupCofoundryCommandDto dto)
        {
            var command = new SetupCofoundryCommand()
            {
                ApplicationName = dto.ApplicationName,
                UserEmail = dto.UserEmail,
                UserFirstName = dto.UserFirstName,
                UserLastName = dto.UserLastName,
                UserPassword = dto.UserPassword
            };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}