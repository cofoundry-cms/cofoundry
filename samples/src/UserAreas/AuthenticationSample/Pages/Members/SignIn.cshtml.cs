using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members;

public class SignInModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public SignInModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [BindProperty]
    [DataType(DataType.EmailAddress)]
    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please provide your password")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var isSignedIn = await _contentRepository
            .Users()
            .Current()
            .IsSignedIn()
            .ExecuteAsync();

        if (isSignedIn)
        {
            return RedirectToPage("Index");
        }

        return Page();
    }

    /// <summary>
    /// In this sample members are registered in the admin panel
    /// which generates a temporary password that needs to be changed
    /// at first sign in. This means that we need to identity that a
    /// password change is required before signing a user in and redirect
    /// them to the change password page.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // First authenticate the user without signing them in
        var authResult = await _contentRepository
            .WithModelState(this)
            .Users()
            .Authentication()
            .AuthenticateCredentials(new AuthenticateUserCredentialsQuery()
            {
                UserAreaCode = MemberUserArea.Code,
                Username = Email,
                Password = Password
            })
            .ExecuteAsync();

        if (!ModelState.IsValid || !authResult.IsSuccess)
        {
            // If the result isn't successful, the the ModelState will be populated
            // with an an error, but you could ignore ModelState handling and
            // instead add your own custom error views/messages by using authResult directly
            return Page();
        }

        // We may need to return to the page that was the source of the access attmpt.
        // This helper will retreive the url from the querystring and ensure it
        // is valid for a redirect
        var returnUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);

        // If a password change is required, redirect the user
        if (authResult.User.RequirePasswordChange)
        {
            return RedirectToPage("PasswordChangeRequired", new { returnUrl });
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

        return RedirectToPage("./Index");
    }
}
