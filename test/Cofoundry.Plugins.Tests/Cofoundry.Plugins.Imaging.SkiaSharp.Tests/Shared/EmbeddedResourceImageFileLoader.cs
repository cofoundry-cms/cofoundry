using System.Reflection;

namespace Cofoundry.Plugins.Imaging.SkiaSharp.Tests.Shared;

public static class EmbeddedResourceImageFileLoader
{
    private static readonly Assembly _assembly = typeof(EmbeddedResourceImageFileLoader).Assembly;

    public static ImageFileSource Load(string filename)
    {
        var path = _assembly.GetName().Name + ".TestImages." + filename;
        var fileStream = _assembly.GetManifestResourceStream(path);

        if (fileStream == null)
        {
            throw new FileLoadException($"Could not load embedded file at path '{path}'");
        }

        return ImageFileSource.Load(fileStream);
    }
}
