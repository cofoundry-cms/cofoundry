using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetImageAssetDetailsByIdQuery : IQuery<ImageAssetDetails>
    {
        public GetImageAssetDetailsByIdQuery()
        {
        }

        public GetImageAssetDetailsByIdQuery(int imageAssetId)
        {
            ImageAssetId = imageAssetId;
        }

        public int ImageAssetId { get; set; }
    }
}
