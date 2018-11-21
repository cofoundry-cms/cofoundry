using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// A url library specifically fo use with admin mail templates.
    /// </summary>
    public class AdminMailTemplateUrlLibrary
    {
        private readonly AdminSettings _adminSettings;

        public AdminMailTemplateUrlLibrary(
            AdminSettings adminSettings
            )
        {
            _adminSettings = adminSettings;
        }

        public string Login()
        {
            return "/" + _adminSettings.DirectoryName;
        }

        public string PasswordReset(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            return string.Format("{0}/auth/reset-password?i={1}&t={2}",
                _adminSettings.DirectoryName,
                context.UserPasswordResetRequestId.ToString("N"),
                Uri.EscapeDataString(context.Token));
        }
    }
}