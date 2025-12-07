using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members.MyAccount;

[AuthorizeUserArea(MemberUserArea.Code)]
public class DeleteAccountModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public DeleteAccountModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

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
            .DeleteAsync();

        IsSuccess = ModelState.IsValid;
    }
}