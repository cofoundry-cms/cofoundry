using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="ICofoundryHtmlHelper"/>.
/// </summary>
public class CofoundryHtmlHelper : ICofoundryHtmlHelper
{
    /// <inheritdoc/>
    public IHtmlContent ClassIf(bool condition, string cls, string? elseCls = null)
    {
        var classToAdd = condition ? cls : elseCls;

        if (string.IsNullOrEmpty(classToAdd))
        {
            return HtmlString.Empty;
        }

        return new HtmlString($"class=\"{elseCls}\"");
    }

    /// <inheritdoc/>
    public IHtmlContent TextIf(bool condition, string passContent, string? failContent = null)
    {
        if (condition)
        {
            return new HtmlString(passContent);
        }
        else if (failContent != null)
        {
            return new HtmlString(failContent);
        }

        return HtmlString.Empty;
    }
}
