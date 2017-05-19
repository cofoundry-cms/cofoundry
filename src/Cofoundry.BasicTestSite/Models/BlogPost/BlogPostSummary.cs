using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class BlogPostSummary
    {
        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public ImageAssetRenderDetails ThumbnailImageAsset { get; set; }

        public string FullPath { get; set; }

        public DateTime PostDate { get; set; }
    }
}