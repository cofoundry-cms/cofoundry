using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    public class PartnerMailTemplatePath
    {
        private const string Root = "~/Cofoundry/MailTemplates/PartnerUsers/";
        private const string LayoutPath = Root + "Layouts/";

        public const string LayoutPath_Html = LayoutPath + "_PartnerMailLayout_html.cshtml";
        public const string LayoutPath_Text = LayoutPath + "_PartnerMailLayout_text.cshtml";

        public static string TemplateView(string viewName)
        {
            return Root + "Templates/" + viewName;
        }
    }
}