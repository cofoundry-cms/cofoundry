using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public static class UrlLibrary
    {
        public static string PartnerLogin(string returnUrl = null)
        {
            var url = "/partner/auth/login";

            if (returnUrl != null)
            {
                url += "?returnUrl=" + WebUtility.UrlEncode(returnUrl);
            }

            return url;
        }

        public static string PartnerDefault()
        {
            return "/partner/welcome";
        }
    }
}
