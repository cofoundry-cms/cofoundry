using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// How to resolve aspect ratio differences between the requested size and the
    /// original image's size.
    /// </summary>
    /// <remarks>
    /// Baed on ImageResizer.FitMode, but with some options removed to reduce the potential
    /// implementation surface area.
    /// </remarks>
    public enum ImageFitMode
    {
        /// <summary>
        /// Fit mode will be determined by other settings, such as &carve=true, &stretch=fill,
        /// and &crop=auto. If none are specified and width/height are specified , &mode=pad
        /// will be used. If maxwidth/maxheight are used, &mode=max will be used.
        /// </summary>
        None = 0,

        /// <summary>
        /// Width and height are considered maximum values. The resulting image may be
        /// smaller to maintain its aspect ratio. The image may also be smaller if the
        /// source image is smaller
        /// </summary>
        Max = 1,

        /// <summary>
        /// Width and height are considered exact values - padding is used if there is
        /// an aspect ratio difference.
        /// </summary>
        Pad = 2,

        /// <summary>
        /// Width and height are considered exact values - cropping is used if there
        /// is an aspect ratio difference.
        /// </summary>
        Crop = 3,

        /// <summary>
        /// Width and height are considered exact values - seam carving is used if there
        /// is an aspect ratio difference. Requires the SeamCarving plugin to be installed,
        /// otherwise behaves like 'pad'.
        /// </summary>
        //Carve = 4,

        /// <summary>
        /// Width and height are considered exact values - if there is an aspect ratio
        /// difference, the image is stretched.
        /// </summary>
        //Stretch = 5,
    }
}
