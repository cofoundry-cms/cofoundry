using Microsoft.AspNetCore.Mvc;

namespace RegistrationAndVerificationSample.Controllers;

[Route("members/auth")]
[AutoValidateAntiforgeryToken]
public class MembersAuthController : Controller
{
    private readonly IAdvancedContentRepository _contentRepository;

    public MembersAuthController(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var isSignedIn = await _contentRepository
            .Users()
            .Current()
            .IsSignedIn()
            .ExecuteAsync();

        if (isSignedIn)
        {
            return RedirectToAction("Index", "Members");
        }

        var viewModel = new SignInViewModel();
        return View(viewModel);
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(SignInViewModel viewModel)
    {
        // First authenticate the user without signing them in
        var authResult = await _contentRepository
            .WithModelState(this)
            .Users()
            .Authentication()
            .AuthenticateCredentials(new AuthenticateUserCredentialsQuery()
            {
                UserAreaCode = MemberUserArea.Code,
                Username = viewModel.Username,
                Password = viewModel.Password
            })
            .ExecuteAsync();

        if (!ModelState.IsValid || !authResult.IsSuccess)
        {
            // If the result isn't successful, the the ModelState will be populated
            // with an an error, but you could ignore ModelState handling and
            // instead add your own custom error views/messages by using authResult directly.
            // "authResult.IsSuccess" is only referenced to improve nullable type handling in subsequent code.
            return View(viewModel);
        }

        // We may need to return to the page that was the source of the access attmpt.
        // This helper will retreive the url from the querystring and ensure it
        // is valid for a redirect
        var returnUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);

        // If a password change is required, redirect the user
        if (authResult.User.RequirePasswordChange)
        {
            // ...omitted: See AuthenticationSample for an example of RequirePasswordChange
        }

        // If not yet verified, redirect the user
        if (!authResult.User.IsAccountVerified)
        {
            return View("VerificationRequired");
        }

        // If no action required: sign the user in
        await _contentRepository
            .Users()
            .Authentication()
            .SignInAuthenticatedUserAsync(new SignInAuthenticatedUserCommand()
            {
                UserId = authResult.User.UserId,
                RememberUser = true
            });

        // Success: redirect the signed in user
        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Members");
    }

    [HttpPost("SignOut")]
    public async Task<IActionResult> SignOutUser()
    {
        await _contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();

        return View();
    }
}
