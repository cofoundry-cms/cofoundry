namespace Cofoundry.Domain.Extendable;

/// <summary>
/// Functionality to be used for extension only e.g. internally 
/// by Cofoundry, in plugins or custom extensions. Access members by referencing the
/// <see cref="Cofoundry.Domain.Extendable"/> namespace and invoking the 
/// <see cref="IExtendablePermissionSetBuilderExtensions.AsExtendableBuilder"/> method on your
/// builder instance.
/// </summary>
public interface IExtendablePermissionSetBuilder : IPermissionSetBuilder
{
    /// <summary>
    /// Service provider instance to be used for extension only e.g. internally 
    /// by Cofoundry, plugins or custom extensions.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}
