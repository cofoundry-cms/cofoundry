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
    public class EmailTemplateBase : TemplateBase
    {
        public EmailTemplateBase(
            ISiteUriResolver siteUriResolver
            )
        {
            Html = new RazorEngineHtmlHelper();
            Url = new RazorEngineUrlHelper(siteUriResolver);
        }

        public RazorEngineHtmlHelper Html { get; set; }

        public RazorEngineUrlHelper Url { get; set; }
    }

    public class EmailTemplateBase<T> : TemplateBase<T>
    {
        public EmailTemplateBase(
            ISiteUriResolver siteUriResolver
            )
        {
            Html = new RazorEngineHtmlHelper();
            Url = new RazorEngineUrlHelper(siteUriResolver);
        }

        public RazorEngineHtmlHelper Html { get; set; }

        public RazorEngineUrlHelper Url { get; set; }
    }
}
