using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetRenderDetails objects.
    /// </summary>
    public interface IImageAssetRenderDetailsMapper
    {
        /// <summary>
        /// Maps an EF ImageAsset record from the db into a ImageAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAsset record from the database.</param>
        ImageAssetRenderDetails Map(ImageAsset dbImage);
    }
}
