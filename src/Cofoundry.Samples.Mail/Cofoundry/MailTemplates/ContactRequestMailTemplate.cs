using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.Mail
{
    public class ContactRequestMailTemplate : IMailTemplate
    {
        public string ViewFile => "~/Views/EmailTemplates/ContactRequest";

        public string Subject => "New Contact Request";

        public ContactRequest Request { get; set; }
    }
}