namespace Cofoundry.Domain.MailTemplates;

/// <summary>
/// A simple helper class to make it easier to work
/// with paths in the default mail template view files.
/// </summary>
public class DefaultMailTemplatePath
{
    private const string Root = "~/MailTemplates/Templates/Default/";

    /// <summary>
    /// Root path to the layout view without the file type postfix.
    /// </summary>
    public const string LayoutPath = Root + "Layouts/_MailLayout";

    /// <summary>
    /// Path to the layout view for html mail templates.
    /// </summary>
    public const string HtmlTemplateLayoutPath = LayoutPath + "_html.cshtml";

    /// <summary>
    /// Path to the layout file for plain text mail templates.
    /// </summary>
    public const string TextTemplateLayoutPath = LayoutPath + "_text.cshtml";

    /// <summary>
    /// Formats the path to a view file in the DefautMailTemplates folder.
    /// </summary>
    /// <param name="viewName">
    /// Name of the view without the html/text postfix or file 
    /// extension e.g. "MyTemplateName".
    /// </param>
    public static string TemplateView(string viewName)
    {
        return Root + viewName;
    }
}
