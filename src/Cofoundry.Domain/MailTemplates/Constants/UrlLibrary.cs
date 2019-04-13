using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates
{
    public class UrlLibrary
    {
        private const string ADMIN_ROOT = "~/admin";

        public static readonly string Login = ADMIN_ROOT;

        public static string PasswordReset(IResetPasswordTemplate template)
        {
            return PasswordReset(template, ADMIN_ROOT);
        }

        private static string PasswordReset(IResetPasswordTemplate template, string basePath)
        {
            return string.Format("{0}/auth/reset-password?i={1}&t={2}",
                basePath,
                template.UserPasswordResetRequestId.ToString().Replace("-", string.Empty),
                Uri.EscapeDataString(template.Token));
        }

        /// <summary>
        /// Temporary version to fix issue with non-standard admin path. This
        /// will be refactored in the next update.
        /// </summary>
        public static string PasswordReset(IResetPasswordTemplate template, IConfiguration configuration)
        {
            return PasswordReset(template, GetAdminPath(configuration));
        }

        private static string GetAdminPath(IConfiguration configuration)
        {
            const string defaultPath = "admin";

            var path = configuration.GetValue<string>("Cofoundry:Admin:DirectoryName");

            return "~/" + (string.IsNullOrWhiteSpace(path) ? defaultPath : path);
        }

        /// <summary>
        /// Temporary version to fix issue with non-standard admin path. This
        /// will be refactored in the next update.
        /// </summary>
        public static string LoginUrl(IConfiguration configuration)
        {
            return GetAdminPath(configuration);
        }
    }
}