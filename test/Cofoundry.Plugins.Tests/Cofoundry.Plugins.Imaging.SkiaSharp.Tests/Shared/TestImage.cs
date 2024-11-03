using Cofoundry.Plugins.Imaging.SkiaSharp.Tests.Shared;

namespace Cofoundry.Plugins.Imaging.SkiaSharp.Tests;

public class TestImage
{
    public required string FileName { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public ImageFileSource Load()
    {
        return EmbeddedResourceImageFileLoader.Load(FileName);
    }
}
