using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members;

public class ForgotPasswordModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public ForgotPasswordModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [BindProperty]
    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    public string Email { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }

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

    public async Task OnPostAsync()
    {
        await _contentRepository
            .WithModelState(this)
            .Users()
            .AccountRecovery()
            .InitiateAsync(new InitiateUserAccountRecoveryViaEmailCommand()
            {
                UserAreaCode = MemberUserArea.Code,
                Username = Email
            });

        IsSuccess = ModelState.IsValid;
    }
}
