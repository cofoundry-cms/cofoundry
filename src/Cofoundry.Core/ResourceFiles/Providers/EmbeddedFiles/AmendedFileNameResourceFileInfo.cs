using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Core.ResourceFiles;

/// <summary>
/// A wrapper IFileInfo implementation that allows the filename to be modified. This
/// is to get around inconsistency issues with the EmbeddedResourceFileInfo which uses
/// the full path of the resource as the name which is inconsistent with the bahaviour
/// of PhysicalResourceFileInfo.
/// </summary>
public class AmendedFileNameResourceFileInfo : IFileInfo
{
    private readonly IFileInfo _fileInfo;

    public AmendedFileNameResourceFileInfo(string fileName, IFileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(fileInfo);

        _fileInfo = fileInfo;
        Name = fileName;
    }

    public bool Exists => _fileInfo.Exists;

    public long Length => _fileInfo.Length;

    public string PhysicalPath => _fileInfo.PhysicalPath;

    public string Name { get; private set; }

    public string OriginalName => _fileInfo.Name;

    public DateTimeOffset LastModified => _fileInfo.LastModified;

    public bool IsDirectory => _fileInfo.IsDirectory;

    public Stream CreateReadStream()
    {
        return _fileInfo.CreateReadStream();
    }
}
