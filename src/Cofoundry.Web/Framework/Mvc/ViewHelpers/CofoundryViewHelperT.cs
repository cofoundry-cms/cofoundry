using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class CofoundryHelper<T>
    {
        public CofoundryHelper(HtmlHelper htmlHelper, T model)
        {
            // DI because mvc framework doesn't support injection yet
            Routing = IckyDependencyResolution.ResolveFromMvcContext<IRouteViewHelper>();
            Settings = IckyDependencyResolution.ResolveFromMvcContext<ISettingsViewHelper>();
            CurrentUser = IckyDependencyResolution.ResolveFromMvcContext<ICurrentUserViewHelper>();
            Js = IckyDependencyResolution.ResolveFromMvcContext<IJavascriptViewHelper>(); ;
            UI = new UIViewHelper<T>(htmlHelper, model);
            Sanitizer = IckyDependencyResolution.ResolveFromMvcContext<IHtmlSanitizerHelper>(); ;
            Html = IckyDependencyResolution.ResolveFromMvcContext<ICofoundryHtmlHelper>(); ;

            Model = model;
        }

        public IRouteViewHelper Routing { get; private set; }

        public ISettingsViewHelper Settings { get; private set; }

        public ICurrentUserViewHelper CurrentUser { get; private set; }

        public IUIViewHelper<T> UI { get; private set; }

        public IJavascriptViewHelper Js { get; private set; }

        public IHtmlSanitizerHelper Sanitizer { get; private set; }

        public ICofoundryHtmlHelper Html { get; set; }

        public T Model { get; private set; }
    }
}
