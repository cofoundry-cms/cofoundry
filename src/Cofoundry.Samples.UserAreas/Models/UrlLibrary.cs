using System.Net;

namespace Cofoundry.Samples.UserAreas
{
    public static class UrlLibrary
    {
        public static string CustomerRoot()
        {
            return "/partners";
        }

        public static string CustomerDefault()
        {
            return CustomerRoot();
        }

        public static string CustomerLogin(string returnUrl = null)
        {
            var url = CustomerRoot() + "/auth/login";

            if (returnUrl != null)
            {
                url += "?returnUrl=" + WebUtility.UrlEncode(returnUrl);
            }

            return url;
        }

        public static string CustomerForgotPassword()
        {
            return CustomerRoot() + "/auth/forgot-password";
        }

        public static string CustomerSignOut()
        {
            return PartnerRoot() + "/auth/logout";
        }

        public static string PartnerRoot()
        {
            return "/partners";
        }

        public static string PartnerSignIn(string returnUrl = null)
        {
            var url = PartnerRoot() + "/auth/login";

            if (returnUrl != null)
            {
                url += "?returnUrl=" + WebUtility.UrlEncode(returnUrl);
            }

            return url;
        }

        public static string PartnerForgotPassword()
        {
            return PartnerRoot() + "/auth/forgot-password";
        }

        public static string PartnerSignOut()
        {
            return PartnerRoot() + "/auth/logout";
        }

        public static string PartnerDefault()
        {
            return PartnerRoot();
        }

        public static string PartnerWelcome()
        {
            return PartnerRoot() + "/welcome";
        }
    }
}
