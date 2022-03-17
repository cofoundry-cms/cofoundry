namespace Cofoundry.Core.ResourceFiles;

/// <summary>
/// Represents a file, which would be an embedded resource in an assembly or
/// a physical file on disk
/// </summary>
public interface IResourceFile
{
    /// <summary>
    /// Returns a read-only stream to the resource.
    /// </summary>
    Stream Open();

    /// <summary>
    /// The name of the file including the file extension
    /// </summary>
    string Name { get; }

    string VirtualPath { get; }
}
