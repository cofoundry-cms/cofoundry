using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class RichTextDisplayModel : IPageBlockTypeDisplayModel
{
    public HtmlString RawHtml { get; set; }
}
