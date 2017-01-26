using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A collection of settings thats can be used to dynamically resize an image. Based
    /// on ImageResizer.ResizerSettings.
    /// </summary>
    public interface IImageResizeSettings
    {
        /// <summary>
        /// How to anchor the image when cropping or adding whitespace to meet sizing
        /// requirements.
        /// </summary>
        ImageAnchorLocation Anchor { get; set; }

        /// <summary>
        /// Hex color to use as the background color if the image is padded
        /// </summary>
        string BackgroundColor { get; set; }

        /// <summary>
        /// Sets the desired height of the image. (minus padding, borders,
        /// margins, effects, and rotation) The only instance the resulting image will
        /// be smaller is if the original source image is smaller. Set Scale=Both to
        /// upscale these images and ensure the output always matches 'width' and 'height'.
        /// If both width and height are specified, the image will be 'letterboxed' to
        /// match the desired aspect ratio. Change the Mode property to adjust this behavior.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Sets the fit mode for the image. max, min, pad, crop, carve, stretch
        /// </summary>
        ImageFitMode Mode { get; set; }

        /// <summary>
        ///  Whether to downscale, upscale, upscale the canvas, or both upscale
        ///  or downscale the image as needed. Defaults to DownscaleOnly.
        /// </summary>
        ImageScaleMode Scale { get; set; }

        /// <summary>
        /// Sets the desired width of the image. (minus padding, borders,
        /// margins, effects, and rotation). The only instance the resulting image will
        /// be smaller is if the original source image is smaller. Set Scale=Both to
        /// upscale these images and ensure the output always matches 'width' and 'height'.
        /// If both width and height are specified, the image will be 'letterboxed' to
        /// match the desired aspect ratio. Change the Mode property to adjust this behavior.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets a querystring that represent this set of settings in the format ?w=20&amp;h=30
        /// </summary>
        string ToQueryString();
    }
}
