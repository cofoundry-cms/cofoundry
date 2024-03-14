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
        string formatStr = "class=\"{0}\"";

        if (condition)
        {
            return new HtmlString(string.Format(formatStr, cls));
        }
        else if (elseCls != null)
        {
            return new HtmlString(string.Format(formatStr, elseCls));
        }

        return HtmlString.Empty;
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
