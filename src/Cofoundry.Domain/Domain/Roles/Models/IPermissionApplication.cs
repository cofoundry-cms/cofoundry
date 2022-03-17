namespace Cofoundry.Domain;

/// <summary>
/// Represemts a permission applied to an object. Typically this can just be
/// an IPermission object, but permissions could be applied in otherways, e.g.
/// in an OR relationship using CompositePermissionApplication
/// </summary>
public interface IPermissionApplication
{
}
