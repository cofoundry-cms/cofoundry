using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class UsersApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UsersApiController(
            IApiResponseHelper apiResponseHelper,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _apiResponseHelper = apiResponseHelper;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public async Task<JsonResult> Get([FromQuery] SearchUserSummariesQuery query)
        {
            if (query == null) query = new SearchUserSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public async Task<JsonResult> GetById(int userId)
        {
            var query = new GetUserDetailsByIdQuery(userId);
            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public async Task<JsonResult> Post([FromBody] AddUserCommand command)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
            if (userArea.AllowPasswordSignIn)
            {
                return await _apiResponseHelper.RunCommandAsync(new AddUserWithTemporaryPasswordCommand()
                {
                    Email = command.Email,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    RoleCode = command.RoleCode,
                    RoleId = command.RoleId,
                    UserAreaCode = command.UserAreaCode,
                    Username = command.Username,
                    DisplayName = command.DisplayName
                });
            }

            return await _apiResponseHelper.RunCommandAsync(command);
        }

        public async Task<JsonResult> PostWithTemporaryPassword([FromBody] AddUserWithTemporaryPasswordCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Patch(int userId, [FromBody] IDelta<UpdateUserCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(userId, delta);
        }

        public Task<JsonResult> PutResetPassword(int userId)
        {
            var command = new ResetUserPasswordCommand() { UserId = userId };
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;

            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}