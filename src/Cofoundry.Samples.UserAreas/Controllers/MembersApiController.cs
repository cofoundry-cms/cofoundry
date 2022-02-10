using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas.Controllers
{
    [Route("api/members")]
    [ApiController]
    public class MembersApiController : ControllerBase
    {
        private readonly IAdvancedContentRepository _contentRepository;

        public MembersApiController(
            IAdvancedContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            var validationResult = await _contentRepository
                .WithModelState(this)
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                    Token = registerUserDto.Token
                })
                .ExecuteAsync();

            // WithModelState is used in the query to capture any errors and add them 
            // to ModelState, which can simplify our action
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Registering a user and marking the task complete should 
            // be run in a transaction
            using (var scope = _contentRepository.Transactions().CreateScope())
            {
                // Register user 
                // (ommited)

                // Mark task completed
                await _contentRepository
                    .WithModelState(this)
                    .AuthorizedTasks()
                    .CompleteAsync(new CompleteAuthorizedTaskCommand()
                    {
                        AuthorizedTaskId = validationResult.Data.AuthorizedTaskId
                    });

                await scope.CompleteIfValidAsync(ModelState);
            }

            return Ok();
        }
    }
}
