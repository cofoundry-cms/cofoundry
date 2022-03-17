using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace Cofoundry.Web;

public class JavascriptViewHelper : IJavascriptViewHelper
{
    public IHtmlContent ToJson<T>(T value)
    {
        var valueJson = JsonConvert.SerializeObject(value);

        return new HtmlString(valueJson);
    }
}
