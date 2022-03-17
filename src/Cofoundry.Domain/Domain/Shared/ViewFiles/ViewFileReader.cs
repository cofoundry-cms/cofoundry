using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class ViewFileReader : IViewFileReader
{
    private readonly IResourceLocator _resourceLocator;

    public ViewFileReader(
        IResourceLocator resourceLocator
        )
    {
        _resourceLocator = resourceLocator;
    }

    public virtual async Task<string> ReadViewFileAsync(string path)
    {
        string result = null;

        if (!FileExists(path)) return result;

        var file = _resourceLocator.GetFile(path);
        if (file == null || !file.Exists || file.IsDirectory) return null;

        using (var stream = file.CreateReadStream())
        using (var reader = new StreamReader(stream))
        {
            result = await reader.ReadToEndAsync();
        }

        return result;
    }

    protected bool FileExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        // check well formatted path
        if (path[0] != '~' && path[0] != '/') return false;

        return _resourceLocator.FileExists(path);
    }
}