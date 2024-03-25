namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Order UpdatePackage instances into the correct execution order, making
/// sure dependencies are taken into consideration.
/// </summary>
public interface IUpdatePackageOrderer
{
    /// <summary>
    /// Orders a collection of UpdatePackage instances into the correct execution 
    /// order, making sure dependencies are taken into consideration.
    /// </summary>
    /// <param name="packages">Update packages to sort.</param>
    /// <returns>Collection of update packages, sorted into the correct order.</returns>
    IReadOnlyCollection<UpdatePackage> Order(IReadOnlyCollection<UpdatePackage> packages);
}
