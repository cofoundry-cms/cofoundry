namespace Cofoundry.Domain.Internal;

/// <summary>
/// The default password policy builder for all user areas. This
/// simply calls into <see cref="IPasswordPolicyBuilderExtensions.UseDefaults"/>.
/// </summary>
public class DefaultPasswordPolicyConfiguration : IDefaultPasswordPolicyConfiguration
{
    public void Configure(IPasswordPolicyBuilder builder)
    {
        builder.UseDefaults();
    }
}
