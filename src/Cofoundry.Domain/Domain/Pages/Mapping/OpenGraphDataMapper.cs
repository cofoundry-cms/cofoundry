using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to OpenGraphData objects.
    /// </summary>
    public class OpenGraphDataMapper : IOpenGraphDataMapper
    {
        private readonly IImageAssetRenderDetailsMapper _imageAssetRenderDetailsMapper;

        public OpenGraphDataMapper(
            IImageAssetRenderDetailsMapper imageAssetRenderDetailsMapper
            )
        {
            _imageAssetRenderDetailsMapper = imageAssetRenderDetailsMapper;
        }

        /// <summary>
        /// Maps an EF PageVersion record from the db into an OpenGraphData 
        /// object.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database, must include the OpenGraphImageAsset property.</param>
        public virtual OpenGraphData Map(PageVersion dbPageVersion)
        {
            var result = new OpenGraphData()
            {
                Description = dbPageVersion.OpenGraphDescription,
                Title = dbPageVersion.OpenGraphTitle
            };

            if (dbPageVersion.OpenGraphImageId.HasValue && dbPageVersion.OpenGraphImageAsset == null)
            {
                throw new Exception($"{nameof(PageVersion)}.{nameof(dbPageVersion.OpenGraphImageAsset)} must be included in the EF query if using {nameof(OpenGraphDataMapper)}. ");
            }

            if (dbPageVersion.OpenGraphImageAsset != null)
            {
                result.Image = _imageAssetRenderDetailsMapper.Map(dbPageVersion.OpenGraphImageAsset);
            }

            return result;
        }
    }
}
