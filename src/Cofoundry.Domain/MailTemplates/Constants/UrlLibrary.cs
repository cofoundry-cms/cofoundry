using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Domain.MailTemplates
{
    public class UrlLibrary
    {
        private const string ADMIN_ROOT = "~/admin";

        public static readonly string Login = ADMIN_ROOT;

        public static string PasswordReset(IResetPasswordTemplate template)
        {
            return string.Format("{0}/auth/reset-password?i={1}&t={2}",
                ADMIN_ROOT,
                template.UserPasswordResetRequestId.ToString().Replace("-", string.Empty),
                Uri.EscapeDataString(template.Token));
        }
    }
}