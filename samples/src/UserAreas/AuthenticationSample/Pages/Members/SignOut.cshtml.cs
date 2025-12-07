using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members;

public class SignOutModel : PageModel
{
    private readonly IAdvancedContentRepository _contentRepository;

    public SignOutModel(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var isSignedIn = await _contentRepository
            .Users()
            .Current()
            .IsSignedIn()
            .ExecuteAsync();

        if (isSignedIn)
        {
            return RedirectToPage("./");
        }

        return Page();
    }

    public async Task OnPostAsync()
    {
        await _contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();
    }
}
