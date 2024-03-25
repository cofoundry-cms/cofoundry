namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Interface to describe a class that creates one or more
/// update packages which can be used to update an application. Implement
/// this interface if you want to run update commands on application startup.
/// </summary>
public interface IUpdatePackageFactory
{
    IEnumerable<UpdatePackage> Create(IReadOnlyCollection<ModuleVersion> versionHistory);
}
