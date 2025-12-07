using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members;

public class PasswordChangeRequiredModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public PasswordChangeRequiredModel(
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
    [Required]
    [Display(Name = "Current password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

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

    public bool IsSuccess { get; set; }

    public string? ReturnUrl { get; set; }

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
            .UpdatePasswordByCredentialsAsync(new UpdateUserPasswordByCredentialsCommand()
            {
                UserAreaCode = MemberUserArea.Code,
                Username = Email,
                OldPassword = OldPassword,
                NewPassword = NewPassword
            });

        if (ModelState.IsValid)
        {
            ReturnUrl = RedirectUrlHelper.GetAndValidateReturnUrl(this);
            IsSuccess = true;
        }
    }
}
