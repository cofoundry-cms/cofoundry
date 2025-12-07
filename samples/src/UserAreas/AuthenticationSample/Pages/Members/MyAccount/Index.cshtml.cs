using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members.MyAccount;

[AuthorizeUserArea(MemberUserArea.Code)]
public class IndexModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public IndexModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    [BindProperty]
    [DataType(DataType.EmailAddress)]
    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "Name")]
    public string DisplayName { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }

    public async Task OnGetAsync()
    {
        var member = await _contentRepository
            .Users()
            .Current()
            .Get()
            .AsSummary()
            .ExecuteAsync();
        EntityNotFoundException.ThrowIfNull(member, "Current");

        Email = member.Email!;
        DisplayName = member.DisplayName ?? string.Empty;
    }

    public async Task OnPostAsync()
    {
        // When applying the update we can use the patch overload
        // on UpdateAsync so we only have to update the properties we
        // have data for
        await _contentRepository
            .WithModelState(this)
            .Users()
            .Current()
            .UpdateAsync(c =>
            {
                c.DisplayName = DisplayName;
                c.Email = Email;
            });

        IsSuccess = ModelState.IsValid;
    }
}
