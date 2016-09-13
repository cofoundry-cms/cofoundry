using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ImageAssetFile : IImageAssetRenderable
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

        /// <summary>
        /// A stream containing the contents of the file. This needs
        /// to be disposed of when you've finished with it.
        /// </summary>
        public Stream ContentStream { get; set; }
    }
}
