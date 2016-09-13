using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    public class MailMessageRenderer : IMailMessageRenderer
    {
        private readonly IMailViewRenderer _viewRenderer;

        public MailMessageRenderer(
            IMailViewRenderer viewRenderer
            )
        {
            _viewRenderer = viewRenderer;
        }

        public MailMessage Render(IMailTemplate template, SerializeableMailAddress toAddress)
        {

            var message = new MailMessage();

            message.Subject = template.Subject;
            message.To = toAddress;
            FormatFromAddress(template, message);

            message.TextBody = RenderView(template, "text");
            message.HtmlBody = RenderView(template, "html");

            return message;
        }

        private void FormatFromAddress(IMailTemplate template, MailMessage message)
        {
            if (template is IMailTemplateWithCustomFromAddress)
            {
                message.From = ((IMailTemplateWithCustomFromAddress)template).From;
            }
        }

        private string RenderView(IMailTemplate template, string type)
        {
            var path = string.Format("{0}_{1}.cshtml", template.ViewFile, type);
            string view = null;

            try
            {
                view = _viewRenderer.Render(path, template);
            }
            catch (Exception ex)
            {
                throw new TemplateRenderException(path, template, ex);
            }

            return view;
        }
    }
}
