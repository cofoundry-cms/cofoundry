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
    /// <summary>
    /// Main helper for Cofoundry functionality on pages without a 
    /// model defined. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid poluting 
    /// the global namespace.
    /// </summary>
    public class CofoundryPageHelper
    {
        public CofoundryPageHelper(HtmlHelper htmlHelper)
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

        /// <summary>
        /// Helpers for generating links to Cofoundry content
        /// </summary>
        public IContentRouteLibrary Routing { get; private set; }

        /// <summary>
        /// Helper for accessing configuration settings from a view
        /// </summary>
        public ISettingsViewHelper Settings { get; private set; }

        /// <summary>
        /// Helpers for accessing information about the currently logged in user
        /// </summary>
        public ICurrentUserViewHelper CurrentUser { get; private set; }

        /// <summary>
        /// Helper for accessing Cofoundry UI controls.
        /// </summary>
        public IUIViewHelper UI { get; private set; }

        /// <summary>
        /// Helper for working with javascript from .net code
        /// </summary>
        public IJavascriptViewHelper Js { get; private set; }

        /// <summary>
        /// Helper for sanitizing html before it output to the page. You'd typically
        /// want to use this when rendering out user inputted data which may be 
        /// vulnerable to XSS attacks.
        /// </summary>
        public IHtmlSanitizerHelper Sanitizer { get; private set; }

        /// <summary>
        /// Miscellaneous helper functions to make working work html views easier.
        /// </summary>
        public ICofoundryHtmlHelper Html { get; set; }
    }
}
