using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class RawHtmlDisplayModel : IPageBlockTypeDisplayModel
{
    public HtmlString RawHtml { get; set; }
}
