using System.Diagnostics.CodeAnalysis;
using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Abstraction for a file loaded by SkiaSharp into
/// memory. This abstraction helps cut down on the amount of using 
/// statements required when working with an image, so only this instance
/// needs disposing of and not the child elements.
/// </summary>
public sealed class ImageFileSource : IDisposable
{
    /// <summary>
    /// The original image stream being processed. This is disposed of
    /// when this instance is disposed.
    /// </summary>
    public required Stream OriginalStream { get; init; }

    /// <summary>
    /// Image information loaded from the file header before
    /// the main image is loaded. This is disposed of
    /// when this instance is disposed.
    /// </summary>
    public SKCodec? Codec { get; private set; }

    /// <summary>
    /// The full Bitmap image data loaded from the file. This is 
    /// disposed of when this instance is disposed.
    /// </summary>
    public SKBitmap? Bitmap { get; private set; }

    /// <summary>
    /// Indicated that the codec and all bitmap data has been
    /// loaded successfuly.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Codec))]
    [MemberNotNullWhen(true, nameof(Bitmap))]
    public bool IsLoaded { get; private set; }

    public static ImageFileSource Load(Stream stream)
    {
        var source = new ImageFileSource
        {
            OriginalStream = stream
        };

        using (var managedStream = new SKManagedStream(stream))
        {
            source.Codec = SKCodec.Create(managedStream);

            if (source.Codec != null)
            {
                source.Bitmap = SKBitmap.Decode(source.Codec);
                source.IsLoaded = true;
            }
        }

        return source;
    }

    public void Dispose()
    {
        Bitmap?.Dispose();
        Codec?.Dispose();
        OriginalStream?.Dispose();
    }
}
