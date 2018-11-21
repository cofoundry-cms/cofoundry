using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    public class AdminMailTemplatePath
    {
        public const string Root = "~/MailTemplates/AdminMailTemplates/";
        private const string LayoutPath = Root + "Layouts/";
        
        public const string DefaultLayoutPath_Html = LayoutPath + "_DefaultLayout_Html.cshtml";
        public const string DefaultLayoutPath_Text = LayoutPath + "_DefaultLayout_Text.cshtml";

        public static string TemplateView(string viewName)
        {
            return Root + "Templates/" + viewName;
        }
    }
}