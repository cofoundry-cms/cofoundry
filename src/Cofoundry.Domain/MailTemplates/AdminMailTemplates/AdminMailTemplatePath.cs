using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    public class AdminMailTemplatePath
    {
        public const string Root = "~/MailTemplates/AdminMailTemplates/";
        private const string LayoutPath = Root + "Layouts/";

        /// <summary>
        /// Path to the layout view for html mail templates.
        /// </summary>
        public const string LayoutPath_Html = LayoutPath + "_AdminMailLayout_Html.cshtml";

        /// <summary>
        /// Path to the layout file for plain text mail templates.
        /// </summary>
        public const string LayoutPath_Text = LayoutPath + "_AdminMailLayout_Text.cshtml";

        /// <summary>
        /// Formats the path to a view file in the AdminMailTemplates folder.
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