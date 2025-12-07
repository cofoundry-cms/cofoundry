using Microsoft.AspNetCore.Mvc;

namespace RegistrationAndVerificationSample.Controllers;

[Route("members")]
[AuthorizeUserArea(MemberUserArea.Code)]
public class MembersController : Controller
{
    private readonly IAdvancedContentRepository _contentRepository;

    public MembersController(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var member = await _contentRepository
            .Users()
            .Current()
            .Get()
            .AsMicroSummary()
            .ExecuteAsync();

        return View(member);
    }
}