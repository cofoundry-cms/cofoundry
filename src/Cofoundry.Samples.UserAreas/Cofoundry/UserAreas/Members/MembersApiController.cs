using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
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
            .AuthorizedTasks()
            .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
            {
                AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                Token = registerUserDto.Token
            })
            .ExecuteAsync();

        if (!validationResult.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, validationResult.Error.Message);
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
                .AuthorizedTasks()
                .CompleteAsync(new CompleteAuthorizedTaskCommand()
                {
                    AuthorizedTaskId = validationResult.Data.AuthorizedTaskId
                });

            await scope.CompleteAsync();
        }

        return Ok();
    }
}
}