using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.ImageSharp
{
    public class DefaultImageSharpInitializer : IImageSharpInitializer
    {
        private readonly ImageSharpSettings _imageSharpSettings;

        public DefaultImageSharpInitializer(
            ImageSharpSettings imageSharpSettings
            )
        {
            _imageSharpSettings = imageSharpSettings;
        }

        public void Initialize(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(JpegFormat.Instance, new JpegDecoder()
            {
                IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
            });

            configuration.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
            {
                Quality = _imageSharpSettings.JpegQuality
            });

            configuration.ImageFormatsManager.SetDecoder(GifFormat.Instance, new GifDecoder()
            {
                IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
            });

            configuration.ImageFormatsManager.SetDecoder(PngFormat.Instance, new PngDecoder()
            {
                IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
            });
        }
    }
}
