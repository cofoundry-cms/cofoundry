using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// A simple helper class to make it easier to work
    /// with paths for in partner mail template view files.
    /// </summary>
    public class PartnerMailTemplatePath
    {
        private const string Root = "~/Cofoundry/MailTemplates/PartnerUsers/";
        private const string LayoutPath = Root + "Layouts/";

        /// <summary>
        /// Path to the layout view for html mail templates.
        /// </summary>
        public const string HtmlTemplateLayoutPath = LayoutPath + "_PartnerMailLayout_html.cshtml";

        /// <summary>
        /// Path to the layout file for plain text mail templates.
        /// </summary>
        public const string TextTemplateLayoutPath = LayoutPath + "_PartnerMailLayout_text.cshtml";

        /// <summary>
        /// Formats the path to a view file in the PartnerUsers folder.
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