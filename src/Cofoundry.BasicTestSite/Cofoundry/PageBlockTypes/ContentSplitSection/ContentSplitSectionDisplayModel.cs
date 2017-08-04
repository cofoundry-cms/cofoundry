using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class ContentSplitSectionDisplayModel : IPageBlockTypeDisplayModel
    {
        public string Title { get; set; }

        public IHtmlContent HtmlText { get; set; }

        public ImageAssetRenderDetails Image { get; set; }
    }
}