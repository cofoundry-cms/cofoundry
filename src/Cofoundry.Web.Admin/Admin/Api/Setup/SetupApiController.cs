using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("setup")]
    [ValidateApiAntiForgeryToken]
    public class SetupApiController : Controller
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
        public async Task<IActionResult> Post([FromBody] SetupCofoundryCommandDto dto)
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