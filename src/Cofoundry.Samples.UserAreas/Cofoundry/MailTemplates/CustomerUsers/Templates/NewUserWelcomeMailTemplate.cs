using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public class NewUserWelcomeMailTemplate : IMailTemplate
    {
        public string ViewFile
        {
            get { return "~/Cofoundry/MailTemplates/CustomerUsers/NewUserWelcomeMailTemplate"; }
        }

        public string Subject
        {
            get { return "Welcome to the customer area " + DisplayName; }
        }

        public string DisplayName { get; set; }

        public string EmailVerificationUrl { get; set; }
    }
}
