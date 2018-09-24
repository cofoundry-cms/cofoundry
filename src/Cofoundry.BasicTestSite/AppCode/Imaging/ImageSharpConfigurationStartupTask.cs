using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Imaging.ImageSharp
{
    public class ImageSharpConfigurationStartupTask : IStartupConfigurationTask
    {
        private readonly ImageSharpSettings _imageSharpSettings;

        public ImageSharpConfigurationStartupTask(
            ImageSharpSettings imageSharpSettings
            )
        {
            _imageSharpSettings = imageSharpSettings;
        }

        public int Ordering => (int)StartupTaskOrdering.Early;

        public void Configure(IApplicationBuilder app)
        {
            // Setup some initial config
            var imgConfig = SixLabors.ImageSharp.Configuration.Default;
            imgConfig.ImageFormatsManager.SetEncoder(ImageFormats.Jpeg, new JpegEncoder()
            {
                Quality = _imageSharpSettings.JpegQuality,
                IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
            });
            imgConfig.ImageFormatsManager.SetEncoder(ImageFormats.Gif, new GifEncoder()
            {
                IgnoreMetadata = _imageSharpSettings.IgnoreMetadata
            });
        }
    }
}
