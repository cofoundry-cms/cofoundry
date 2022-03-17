using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class SingleLineTextDisplayModel : IPageBlockTypeDisplayModel
{
    public HtmlString Text { get; set; }
}
