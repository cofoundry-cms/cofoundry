using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when an image is deleted
    /// </summary>
    public class ImageAssetDeletedMessage
    {
        /// <summary>
        /// Id of the image asset that has been deleted
        /// </summary>
        public int ImageAssetId { get; set; }
    }
}
