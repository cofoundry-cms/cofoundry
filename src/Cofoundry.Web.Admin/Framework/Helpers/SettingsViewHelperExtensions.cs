using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public static class SettingsViewHelperExtensions
    {
        public static string GetCompanyName(this ISettingsViewHelper helper)
        {
            return helper.Get<GeneralSiteSettings>().CompanyName;
        }

        public static string GetCurrentDeviceView(this ISettingsViewHelper helper)
        {
            return helper.Get<SiteViewerSettings>().SiteViewerDeviceView;
        }
    }
}