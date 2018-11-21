using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    public class GenericMailTemplatePath
    {
        private const string Root = "~/MailTemplates/GenericMailTemplates/";
        private const string LayoutPath = Root + "Layouts/";

        public const string LayoutPath_Html = LayoutPath + "_GenericMailLayout_html.cshtml";
        public const string LayoutPath_Text = LayoutPath + "_GenericMailLayout_text.cshtml";

        public static string TemplateView(string viewName)
        {
            return Root + "Templates/" + viewName;
        }
    }
}