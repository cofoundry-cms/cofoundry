namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Used to expose extension points for extension methods to
/// <see cref="IPasswordPolicyBuilder"/> without poluting the public 
/// api surface.
/// </summary>
public interface IExtendablePasswordPolicyBuilder : IPasswordPolicyBuilder
{
    public PasswordOptions Options { get; }

    /// <summary>
    /// Service provider instance only to be used for extending the 
    /// <see cref="IPasswordPolicyBuilder"/> with extension methods.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}
