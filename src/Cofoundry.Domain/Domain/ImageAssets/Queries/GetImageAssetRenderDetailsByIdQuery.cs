using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetImageAssetRenderDetailsByIdQuery : IQuery<ImageAssetRenderDetails>
    {
        public GetImageAssetRenderDetailsByIdQuery()
        {
        }

        public GetImageAssetRenderDetailsByIdQuery(int imageAssetId)
        {
            ImageAssetId = imageAssetId;
        }

        public int ImageAssetId { get; set; }
    }
}
