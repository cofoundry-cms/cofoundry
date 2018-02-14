using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetImageAssetFileByIdQuery : IQuery<ImageAssetFile>
    {
        public GetImageAssetFileByIdQuery()
        {
        }

        public GetImageAssetFileByIdQuery(int imageAssetId)
        {
            ImageAssetId = imageAssetId;
        }

        public int ImageAssetId { get; set; }
    }
}
