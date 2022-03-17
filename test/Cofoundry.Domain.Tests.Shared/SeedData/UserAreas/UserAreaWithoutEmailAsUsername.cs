namespace Cofoundry.Domain.Tests.Shared.SeedData;

/// <summary>
/// AllowPasswordSignIn = <see langword="true"/>,
/// UseEmailAsUsername = <see langword="false"/>
/// </summary>
public class UserAreaWithoutEmailAsUsername : IUserAreaDefinition
{
    public const string Code = "NEU";

    public string UserAreaCode => Code;

    public string Name => "Username not email";

    public bool AllowPasswordSignIn => true;

    public bool UseEmailAsUsername => false;

    public string SignInPath => "/weu/sign-in";

    public bool IsDefaultAuthScheme => false;

    public void ConfigureOptions(UserAreaOptions options)
    {
    }
}
