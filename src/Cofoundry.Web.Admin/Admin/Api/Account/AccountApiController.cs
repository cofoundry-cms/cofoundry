using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class AccountApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public AccountApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get()
        {
            var query = new GetCurrentUserDetailsQuery();
            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public async Task<JsonResult> Patch([FromBody] IDelta<UpdateCurrentUserCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(delta);
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