using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class ContactRequestMailTemplate : IMailTemplate
    {
        public string ViewFile => "~/Views/EmailTemplates/ContactRequest";

        public string Subject => "New Contact Request";

        public ContactRequest Request { get; set; }
    }
}