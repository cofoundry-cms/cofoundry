using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members;

public class ResetPasswordModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public ResetPasswordModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [BindProperty]
    [Required]
    [Display(Name = "New password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "Confirm new password")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Password does not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;

    public AuthorizedTaskTokenValidationResult? TokenValidationResult { get; set; }

    public bool IsSuccess { get; set; }

    /// <param name="t">
    /// An account reecovery request is authorized using a token
    /// which is included in the URL as a query parameter
    /// named "t". You can access this parameter a number of ways
    /// but here we simply bind it as a parameter to the handler.
    /// </param>
    public async Task<IActionResult> OnGetAsync(string t)
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

        TokenValidationResult = await _contentRepository
            .Users()
            .AccountRecovery()
            .Validate(new ValidateUserAccountRecoveryByEmailQuery()
            {
                UserAreaCode = MemberUserArea.Code,
                Token = t
            })
            .ExecuteAsync();

        return Page();
    }

    public async Task OnPostAsync(string t)
    {
        await _contentRepository
            .WithModelState(this)
            .Users()
            .AccountRecovery()
            .CompleteAsync(new CompleteUserAccountRecoveryViaEmailCommand()
            {
                UserAreaCode = MemberUserArea.Code,
                Token = t,
                NewPassword = NewPassword
            });

        IsSuccess = ModelState.IsValid;
    }
}
