using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class RichTextWithMediaDisplayModel : IPageBlockTypeDisplayModel
{
    public HtmlString RawHtml { get; set; }
}
