namespace Cofoundry.Domain.MailTemplates;

public class AdminMailTemplatePath
{
    public const string Root = "~/MailTemplates/Templates/Admin/";

    /// <summary>
    /// Root path to the layout view without the file type postfix.
    /// </summary>
    public const string LayoutPath = Root + "Layouts/_AdminMailLayout";

    /// <summary>
    /// Path to the layout view for html mail templates.
    /// </summary>
    public const string HtmlTemplateLayoutPath = LayoutPath + "_html.cshtml";

    /// <summary>
    /// Path to the layout file for plain text mail templates.
    /// </summary>
    public const string TextTemplateLayoutPath = LayoutPath + "_text.cshtml";

    /// <summary>
    /// Formats the path to a view file in the admin mail templates folder.
    /// </summary>
    /// <param name="viewName">
    /// Name of the view without the html/text postfix or file 
    /// extension e.g. "MyTemplateName".
    /// </param>
    public static string TemplateView(string viewName)
    {
        return Root + "Admin" + viewName;
    }
}
