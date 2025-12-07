using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members.MyAccount;

[AuthorizeUserArea(MemberUserArea.Code)]
public class UpdatePasswordModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public UpdatePasswordModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

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

    public void OnGet()
    {
    }

    public async Task OnPostAsync()
    {
        await _contentRepository
            .WithModelState(this)
            .Users()
            .Current()
            .UpdatePasswordAsync(new UpdateCurrentUserPasswordCommand()
            {
                OldPassword = OldPassword,
                NewPassword = NewPassword
            });

        IsSuccess = ModelState.IsValid;
    }
}
