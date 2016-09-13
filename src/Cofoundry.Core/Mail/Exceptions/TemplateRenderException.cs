using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    public class TemplateRenderException : Exception
    {
        const string MESSAGE = "Error rendering mail template";

        public TemplateRenderException()
            : base(MESSAGE)
        {
        }

        public TemplateRenderException(string templatePath, IMailTemplate template, Exception innerEx)
            : base(MESSAGE, innerEx)
        {
            TemplatePath = templatePath;
            Template = template;
        }

        public string TemplatePath { get; set; }

        public IMailTemplate Template { get; set; }
    }
}
