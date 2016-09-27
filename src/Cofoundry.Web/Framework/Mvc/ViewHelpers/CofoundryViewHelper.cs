using Cofoundry.Core.Web;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class CofoundryHelper
    {
        public CofoundryHelper(HtmlHelper htmlHelper)
        {
            // DI because mvc framework doesn't support injection yet
            Routing = IckyDependencyResolution.ResolveFromMvcContext<IContentRouteLibrary>();
            Settings = IckyDependencyResolution.ResolveFromMvcContext<ISettingsViewHelper>();
            CurrentUser = IckyDependencyResolution.ResolveFromMvcContext<ICurrentUserViewHelper>();
            Js = IckyDependencyResolution.ResolveFromMvcContext<IJavascriptViewHelper>(); ;
            Sanitizer = IckyDependencyResolution.ResolveFromMvcContext<IHtmlSanitizerHelper>(); ;
            Html = IckyDependencyResolution.ResolveFromMvcContext<ICofoundryHtmlHelper>(); ;
            UI = new UIViewHelper(htmlHelper);
        }

        public IContentRouteLibrary Routing { get; private set; }

        public ISettingsViewHelper Settings { get; private set; }

        public ICurrentUserViewHelper CurrentUser { get; private set; }

        public IUIViewHelper UI { get; private set; }

        public IJavascriptViewHelper Js { get; private set; }

        public IHtmlSanitizerHelper Sanitizer { get; private set; }

        public ICofoundryHtmlHelper Html { get; set; }
    }
}
