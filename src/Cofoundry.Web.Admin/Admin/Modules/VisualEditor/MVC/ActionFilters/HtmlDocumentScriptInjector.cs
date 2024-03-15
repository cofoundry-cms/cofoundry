namespace Cofoundry.Web.Admin.Internal;

/// <summary>
/// Default implementation of <see cref="IHtmlDocumentScriptInjector"/>.
/// </summary>
public class HtmlDocumentScriptInjector : IHtmlDocumentScriptInjector
{
    const string HEAD_TAG_START = "<head>";
    const string HEAD_TAG_END = "</head>";
    const string BODY_TAG_START = "<body";
    const string BODY_TAG_END = "</body>";

    /// <inheritdoc/>
    public string InjectScripts(string html, string? headScript, string? bodyScript)
    {
        html = InjectHeadScript(html, headScript);

        if (string.IsNullOrWhiteSpace(bodyScript))
        {
            // Nothing more to do
            return html;
        }

        var insertBodyIndex = html.LastIndexOf(BODY_TAG_END, StringComparison.OrdinalIgnoreCase);

        // Early return if no body tag found
        if (insertBodyIndex < 0)
        {
            return html;
        }

        html = html.Substring(0, insertBodyIndex)
            + bodyScript
            + Environment.NewLine
            + html.Substring(insertBodyIndex);

        return html;
    }

    private static string InjectHeadScript(string html, string? headScript)
    {
        if (string.IsNullOrWhiteSpace(headScript))
        {
            return html;
        }

        var insertHeadIndex = html.IndexOf(HEAD_TAG_END, StringComparison.OrdinalIgnoreCase);

        if (insertHeadIndex > 0)
        {
            html = html.Substring(0, insertHeadIndex)
                + headScript
                + Environment.NewLine
                + html.Substring(insertHeadIndex);
        }
        else
        {
            // No head, let's add one.
            var bodyStartIndex = html.IndexOf(BODY_TAG_START, StringComparison.OrdinalIgnoreCase);

            if (bodyStartIndex > 0)
            {
                html = html.Substring(0, bodyStartIndex)
                    + HEAD_TAG_START
                    + Environment.NewLine
                    + headScript
                    + Environment.NewLine
                    + HEAD_TAG_END
                    + Environment.NewLine
                    + html.Substring(bodyStartIndex);
            }
        }

        return html;
    }
}
