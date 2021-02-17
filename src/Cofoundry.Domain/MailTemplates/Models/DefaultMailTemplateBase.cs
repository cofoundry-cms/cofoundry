using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public abstract class DefaultMailTemplateBase : IMailTemplate, IMailTemplateWithApplicationName
    {
        /// <summary>
        /// Name or full path to the layout file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/Layouts/_Layout".
        /// </summary>
        public string LayoutFile { get; set; }

        /// <summary>
        /// Name or full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/MyMailTemplate".
        /// </summary>
        public string ViewFile { get; set; }

        /// <summary>
        /// String to use as the subject to the email. To customize this
        /// use the "SubjectFormat" property.
        /// </summary>
        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        /// <summary>
        /// String used to format the email subject. This can optionally 
        /// include a token "{0}" which is replaced with the application 
        /// name configuration setting e.g. "{0}: Password changed".
        /// </summary>
        public string SubjectFormat { get; set; }

        /// <summary>
        /// The application name to use in formatting the subject. By default
        /// this is retreived from a configuration setting.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Formats the layout file path for thr html template by
        /// adding the "_html.cshtml" postfix.
        /// </summary>
        public string GetLayoutFilePathForHtmlTemplate()
        {
            return LayoutFile + "_html.cshtml";
        }

        /// <summary>
        /// Formats the layout file path for thr html template by
        /// adding the "_text.cshtml" postfix.
        /// </summary>
        public string GetLayoutFilePathForTextTemplate()
        {
            return LayoutFile + "_text.cshtml";
        }
    }
}
