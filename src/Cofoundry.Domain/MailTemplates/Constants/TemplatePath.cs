using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates
{
    public class TemplatePath
    {
        public static string Root = "/MailTemplates/AdminMailTemplates/";

        public static string ViewPath = "~" + Root;

        public static string LayoutPath = ViewPath + "Shared/Layouts/";

        public static string DefaultLayoutPath_Html = LayoutPath + "_DefaultLayout_Html.cshtml";
        public static string DefaultLayoutPath_Text = LayoutPath + "_DefaultLayout_Text.cshtml";
    }
}