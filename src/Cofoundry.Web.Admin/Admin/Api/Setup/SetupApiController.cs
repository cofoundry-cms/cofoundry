using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    [AutoValidateAntiforgeryToken]
    public class SetupApiController : Controller
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public SetupApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public Task<JsonResult> Post([FromBody] SetupCofoundryCommandDto dto)
        {
            var command = new SetupCofoundryCommand()
            {
                ApplicationName = dto.ApplicationName,
                UserEmail = dto.UserEmail,
                UserFirstName = dto.UserFirstName,
                UserLastName = dto.UserLastName,
                UserPassword = dto.UserPassword
            };

            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}