﻿using Microsoft.AspNetCore.Html;

namespace Cofoundry.BasicTestSite;

public class ContentSplitSectionDisplayModel : IPageBlockTypeDisplayModel
{
    public string? Title { get; set; }

    public IHtmlContent HtmlText { get; set; } = HtmlString.Empty;

    public ImageAssetRenderDetails? Image { get; set; }
}
