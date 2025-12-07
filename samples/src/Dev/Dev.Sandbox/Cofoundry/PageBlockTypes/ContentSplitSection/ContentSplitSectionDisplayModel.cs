using Microsoft.AspNetCore.Html;

namespace Dev.Sandbox;

public class ContentSplitSectionDisplayModel : IPageBlockTypeDisplayModel
{
    public string? Title { get; set; }

    public IHtmlContent HtmlText { get; set; } = HtmlString.Empty;

    public ImageAssetRenderDetails? Image { get; set; }
}
