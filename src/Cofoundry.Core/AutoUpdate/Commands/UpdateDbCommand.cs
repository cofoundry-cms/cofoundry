namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// A command that executes a sql db script.
/// </summary>
/// <inheritdoc/>
public class UpdateDbCommand : IVersionedUpdateCommand
{
    /// <summary>
    /// The name of the sql file without the .sql extension. For single
    /// object files this will be the name of the object by convention.
    /// </summary>
    public required string FileName { get; set; }

    public required string Sql { get; set; }

    public int Version { get; set; }

    public DbScriptType ScriptType { get; set; }

    public required string Description { get; set; }
}
