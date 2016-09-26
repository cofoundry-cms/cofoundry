using Cofoundry.Core.Web;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// A base template model contianing helper functions for email templating.
    /// </summary>
    public class EmailTemplateBase : TemplateBase, IEmailTemplateWithUrlHelper
    {
        public EmailTemplateBase()
        {
            Html = new RazorEngineHtmlHelper();
        }

        public RazorEngineHtmlHelper Html { get; set; }

        public RazorEngineUrlHelper Url { get; set; }
    }

    /// <summary>
    /// A base template model contianing helper functions for email templating.
    /// </summary>
    public class EmailTemplateBase<T> : TemplateBase<T>, IEmailTemplateWithUrlHelper
    {
        public EmailTemplateBase()
        {
            Html = new RazorEngineHtmlHelper();
        }

        public EmailTemplateBase(
            ISiteUriResolver siteUriResolver
            )
        {
            Url = new RazorEngineUrlHelper(siteUriResolver);
        }

        public RazorEngineHtmlHelper Html { get; set; }

        public RazorEngineUrlHelper Url { get; set; }
    }
}
