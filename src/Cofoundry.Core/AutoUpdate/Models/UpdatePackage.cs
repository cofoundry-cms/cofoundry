namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Represents a set of updates for a particular module.
/// </summary>
public class UpdatePackage
{
    /// <summary>
    /// Unique identifier for the module
    /// </summary>
    public required string ModuleIdentifier { get; set; }

    /// <summary>
    /// Commands to run when updating the module to a specific version
    /// </summary>
    public ICollection<IVersionedUpdateCommand> VersionedCommands { get; set; } = new List<IVersionedUpdateCommand>();

    /// <summary>
    /// Commands to run after all versioned commands have been run
    /// </summary>
    public ICollection<IAlwaysRunUpdateCommand> AlwaysUpdateCommands { get; set; } = new List<IAlwaysRunUpdateCommand>();

    /// <summary>
    /// A collection of module identifiers that this module is dependent on
    /// </summary>
    public ICollection<string> DependentModules { get; set; } = new List<string>();

    /// <summary>
    /// Indicates if this contains one-off versioned updates. This basically
    /// ignores "always update commands" when checking to see if an exception
    /// should be thrown if the db is locked for updates and updates are required. 
    /// </summary>
    public bool ContainsVersionUpdates()
    {
        return !EnumerableHelper.IsNullOrEmpty(VersionedCommands);
    }
}
