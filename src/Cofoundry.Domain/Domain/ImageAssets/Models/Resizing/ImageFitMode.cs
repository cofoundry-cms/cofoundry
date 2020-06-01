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
    public enum ImageFitMode
    {
        /// <summary>
        /// The default fit mode will be used. This can vary depending on the plugin 
        /// being used, but is typically set to Crop.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Width and height are considered maximum values. The resulting image may be
        /// smaller to maintain its aspect ratio.
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
        Crop = 3
    }
}
