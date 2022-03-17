using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Core.ResourceFiles;

/// <summary>
/// A IFileInfo implementation that represents an embedded resource
/// directory.
/// </summary>
public class EmbeddedResourceDirectoryInfo : IFileInfo
{
    public EmbeddedResourceDirectoryInfo(string directoryName, DateTimeOffset lastModified)
    {
        Name = directoryName;
        LastModified = lastModified;
    }

    public bool Exists => true;

    public long Length => -1;

    public string PhysicalPath => null;

    public string Name { get; private set; }

    public DateTimeOffset LastModified { get; private set; }

    public bool IsDirectory => true;

    public Stream CreateReadStream()
    {
        throw new InvalidOperationException("Cannot create a stream for a directory.");
    }
}
