using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Controls whether the image is allowed to upscale, downscale, both, or if
    /// only the canvas gets to be upscaled.
    /// </summary>
    public enum ImageScaleMode
    {
        /// <summary>
        /// The default. Only downsamples images - never enlarges. If an image is smaller
        /// than 'width' and 'height', the image coordinates are used instead.
        /// </summary>
        DownscaleOnly = 0,

        /// <summary>
        /// Only upscales (zooms) images - never downsamples except to meet web.config
        /// restrictions. If an image is larger than 'width' and 'height', the image
        /// coordinates are used instead.
        /// </summary>
        UpscaleOnly = 1,

        /// <summary>
        /// Upscales and downscales images according to 'width' and 'height', within
        /// web.config restrictions.
        /// </summary>
        Both = 2,

        /// <summary>
        /// When the image is smaller than the requested size, padding is added instead 
        /// of stretching the image
        /// </summary>
        UpscaleCanvas = 3,
    }
}
