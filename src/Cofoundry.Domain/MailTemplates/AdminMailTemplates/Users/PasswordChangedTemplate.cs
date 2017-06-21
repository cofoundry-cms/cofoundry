using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Notifies a user that thier password has been changed.
    /// </summary>
    public class PasswordChangedTemplate : IPasswordChangedTemplate
    {
        public PasswordChangedTemplate()
        {
            Subject = "Cofoundry: Password changed";
            ViewFile = TemplatePath.ViewPath + "Users/PasswordChanged";
        }

        public string ViewFile { get; set; }

        public string Subject { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}