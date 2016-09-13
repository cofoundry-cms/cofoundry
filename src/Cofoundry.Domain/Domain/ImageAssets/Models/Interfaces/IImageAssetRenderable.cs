using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IImageAssetRenderable
    {
        int ImageAssetId { get; set; }
        string FileName { get; set; }
        string Extension { get; set; }
        string Title { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        /// <summary>
        /// The default Anchor Location when using dynamic cropping
        /// </summary>
        ImageAnchorLocation? DefaultAnchorLocation { get; set; }
    }
}
