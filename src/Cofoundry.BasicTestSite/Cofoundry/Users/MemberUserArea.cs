namespace Cofoundry.BasicTestSite.Cofoundry;

public class MemberUserArea : IUserAreaDefinition
{
    public const string Code = "MEM";

    public string UserAreaCode => Code;

    public string Name => "Member";

    public bool AllowPasswordSignIn => true;

    public bool UseEmailAsUsername => true;

    public string SignInPath => "/members/auth/signin";

    public bool IsDefaultAuthScheme => true;

    public void ConfigureOptions(UserAreaOptions options)
    {
    }
}
