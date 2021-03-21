using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.Mail
{
    /// <summary>
    /// Each email consists of a template class and a set of
    /// template files. The template class is used as the view 
    /// model and so you can include any custom propeties that
    /// want to make available in the template views.
    /// </summary>
    public class ContactRequestMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this 
        /// is automatically added.
        /// 
        /// You can place your templates wherever you like e.g. in the Cofoudry 
        /// folder as we do here, or in the Views folder.
        /// </summary>
        public string ViewFile => "~/Cofoundry/MailTemplates/ContactRequest";

        /// <summary>
        /// Used as the subject line in the email.
        /// </summary>
        public string Subject => "New Contact Request";

        /// <summary>
        /// Here we add the contact request as a custom property so we can
        /// write it out in our view.
        /// </summary>
        public ContactRequest Request { get; set; }
    }
}