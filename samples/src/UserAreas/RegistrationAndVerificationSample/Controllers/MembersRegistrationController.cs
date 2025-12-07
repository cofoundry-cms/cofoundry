using Microsoft.AspNetCore.Mvc;

namespace RegistrationAndVerificationSample.Controllers;

[Route("members/registration")]
[AutoValidateAntiforgeryToken]
public class MembersRegistrationController : Controller
{
    private readonly IAdvancedContentRepository _contentRepository;

    public MembersRegistrationController(
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

        var viewModel = new RegisterViewModel();
        return View(viewModel);
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(RegisterViewModel viewModel)
    {
        // When executing multiple commands we should run them inside a transaction
        using (var scope = _contentRepository.Transactions().CreateScope())
        {
            // The anonymous user does not have permissinos to add users
            // so we need to run the commands under the system account by
            // calling "WithElevatedPermissions()"
            var userId = await _contentRepository
                .WithElevatedPermissions()
                .WithModelState(this)
                .Users()
                .AddAsync(new AddUserCommand()
                {
                    UserAreaCode = MemberUserArea.Code,
                    RoleCode = MemberRole.Code,
                    Username = viewModel.Username,
                    Password = viewModel.Password,
                    Email = viewModel.Email
                });

            // In this example we require members to validate their account 
            // before we let them sign in. Initiating verification will send an
            // email notification containing a unique link to verify the account
            await _contentRepository
                .WithModelState(this)
                .Users()
                .AccountVerification()
                .EmailFlow()
                .InitiateAsync(new InitiateUserAccountVerificationViaEmailCommand()
                {
                    UserId = userId
                });

            // This helper method will only complete the transaction if 
            // the model state is valid
            await scope.CompleteIfValidAsync(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        return View("RegistrationSuccess", viewModel);
    }

    [Route("Verify")]
    public async Task<IActionResult> Verify(string t)
    {
        await _contentRepository
            .WithModelState(this)
            .Users()
            .AccountVerification()
            .EmailFlow()
            .CompleteAsync(new CompleteUserAccountVerificationViaEmailCommand()
            {
                UserAreaCode = MemberUserArea.Code,
                Token = t
            });

        if (ModelState.IsValid)
        {
            return View("VerifySuccess");
        }

        return View();
    }

    [Route("ResendVerification")]
    public async Task<IActionResult> ResendVerification()
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

        return View(new ResendVerificationViewModel());
    }

    [HttpPost("ResendVerification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationViewModel viewModel)
    {
        // First authorize the request to ensure we are dealing with the
        // owner of the account
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

        // If the result isn't successful, the the ModelState will be populated
        // with an the error. "authResult.IsSuccess" is only referenced to improve
        // nullable type handling in subsequent code.
        if (!ModelState.IsValid || !authResult.IsSuccess)
        {
            return View();
        }

        // Initiating again will generate a new account verification email.
        await _contentRepository
            .WithModelState(this)
            .Users()
            .AccountVerification()
            .EmailFlow()
            .InitiateAsync(new InitiateUserAccountVerificationViaEmailCommand()
            {
                UserId = authResult.User.UserId
            });

        if (ModelState.IsValid)
        {
            return View("ResendVerificationSuccess");
        }

        return View();
    }
}
