using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to OpenGraphData objects.
    /// </summary>
    public class OpenGraphDataMapper : IOpenGraphDataMapper
    {
        private readonly IImageAssetSummaryMapper _imageAssetSummaryMapper;

        public OpenGraphDataMapper(
            IImageAssetSummaryMapper imageAssetSummaryMapper
            )
        {
            _imageAssetSummaryMapper = imageAssetSummaryMapper;
        }

        /// <summary>
        /// Maps an EF PageVersion record from the db into an OpenGraphData 
        /// object.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database.</param>
        public OpenGraphData Map(PageVersion dbPageVersion)
        {
            var result = new OpenGraphData()
            {
                Description = dbPageVersion.OpenGraphDescription,
                Title = dbPageVersion.OpenGraphTitle
            };

            if (result.Image != null)
            {
                result.Image = _imageAssetSummaryMapper.Map(dbPageVersion.OpenGraphImageAsset);
            }

            return result;
        }
    }
}
