using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public static class SettingsViewHelperExtensions
    {
        public static async Task<string> GetApplicationNameAsync(this ISettingsViewHelper helper)
        {
            var settings = await helper.GetAsync<GeneralSiteSettings>();
            return settings.ApplicationName;
        }
    }
}