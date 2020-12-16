using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail.Internal
{
    /// <summary>
    /// Renders the contents of a mail template to html/text strings in preparation for 
    /// sending out as an email.
    /// </summary>
    public class MailMessageRenderer : IMailMessageRenderer
    {
        private readonly IMailViewRenderer _viewRenderer;

        public MailMessageRenderer(
            IMailViewRenderer viewRenderer
            )
        {
            _viewRenderer = viewRenderer;
        }

        /// <summary>
        ///  Renders the contents of a mail template and formats it into a MailMessage
        ///  object that can be used to send out an email.
        /// </summary>
        /// <param name="template">The mail template that describes the data and template information for the email</param>
        /// <param name="toAddress">The address to send the email to.</param>
        /// <returns>Formatted MailMessage</returns>
        public async Task<MailMessage> RenderAsync(IMailTemplate template, MailAddress toAddress)
        {
            var message = new MailMessage();

            message.Subject = template.Subject;
            message.To = toAddress;
            FormatFromAddress(template, message);

            message.TextBody = await RenderViewAsync(template, "text");
            message.HtmlBody = await RenderViewAsync(template, "html");

            return message;
        }

        private void FormatFromAddress(IMailTemplate template, MailMessage message)
        {
            if (template is IMailTemplateWithCustomFromAddress)
            {
                message.From = ((IMailTemplateWithCustomFromAddress)template).From;
            }
        }

        private async Task<string> RenderViewAsync(IMailTemplate template, string type)
        {
            var path = string.Format("{0}_{1}.cshtml", template.ViewFile, type);
            string view = null;

            try
            {
                view = await _viewRenderer.RenderAsync(path, template);
            }
            catch (Exception ex)
            {
                throw new TemplateRenderException(path, template, ex);
            }

            return view.Trim();
        }
    }
}
