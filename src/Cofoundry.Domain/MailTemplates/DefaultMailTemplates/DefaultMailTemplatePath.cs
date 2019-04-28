using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// A simple helper class to make it easier to work
    /// with paths in the default mail template view files.
    /// </summary>
    public class DefaultMailTemplatePath
    {
        private const string Root = "~/MailTemplates/DefaultMailTemplates/";
        private const string LayoutPath = Root + "Layouts/";

        /// <summary>
        /// Path to the layout view for html mail templates.
        /// </summary>
        public const string LayoutPath_Html = LayoutPath + "_DefaultMailLayout_html.cshtml";

        /// <summary>
        /// Path to the layout file for plain text mail templates.
        /// </summary>
        public const string LayoutPath_Text = LayoutPath + "_DefaultMailLayout_text.cshtml";

        /// <summary>
        /// Formats the path to a view file in the DefautMailTemplates folder.
        /// </summary>
        /// <param name="viewName">
        /// Name of the view without the html/text postfix or file 
        /// extension e.g. "MyTemplateName".
        /// </param>
        public static string TemplateView(string viewName)
        {
            return Root + "Templates/" + viewName;
        }
    }
}