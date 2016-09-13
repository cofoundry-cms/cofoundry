using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ImageAssetRenderDetails : IImageAssetRenderable
    {
        public int ImageAssetId { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// The default Anchor Location when using dynamic cropping
        /// </summary>
        public ImageAnchorLocation? DefaultAnchorLocation { get; set; }
    }
}
