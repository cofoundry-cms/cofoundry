using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password
            };

            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}