using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationSample.Pages.Members;

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

    public UserMicroSummary Member { get; set; } = UserMicroSummary.Uninitialized;

    public async Task OnGetAsync()
    {
        var member = await _contentRepository
            .Users()
            .Current()
            .Get()
            .AsMicroSummary()
            .ExecuteAsync();

        EntityNotFoundException.ThrowIfNull(member, "Current");

        Member = member;
    }
}
