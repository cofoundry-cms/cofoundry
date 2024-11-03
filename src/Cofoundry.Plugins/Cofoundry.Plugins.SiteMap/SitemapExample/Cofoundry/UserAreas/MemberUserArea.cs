namespace SitemapExample;

public class MemberUserArea : IUserAreaDefinition
{
    public const string Code = "MEM";

    public bool AllowPasswordSignIn => true;

    public string Name => "Member";

    public bool UseEmailAsUsername => true;

    public string UserAreaCode => Code;

    public string SignInPath => "/member/auth";

    public bool IsDefaultAuthScheme => true;

    public void ConfigureOptions(UserAreaOptions options)
    {
    }
}
