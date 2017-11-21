using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main helper for Cofoundry functionality on pages without a 
    /// model defined. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid poluting 
    /// the global namespace.
    /// </summary>
    public class CofoundryPageHelper : ICofoundryHelper
    {
        public CofoundryPageHelper(
            IContentRouteLibrary contentRouteLibrary,
            IStaticFileViewHelper staticFileViewHelper,
            ISettingsViewHelper settings,
            ICurrentUserViewHelper currentUser,
            IJavascriptViewHelper js,
            IHtmlSanitizerHelper sanitizer,
            ICofoundryHtmlHelper html
            )
        {
            Routing = contentRouteLibrary;
            StaticFiles = staticFileViewHelper;
            Settings = settings;
            CurrentUser = currentUser;
            Js = js;
            Sanitizer = sanitizer;
            Html = html;
        }

        /// <summary>
        /// Helpers for generating links to Cofoundry content
        /// </summary>
        public IContentRouteLibrary Routing { get; }

        /// <summary>
        /// Helper for resolving urls to static files.
        /// </summary>
        public IStaticFileViewHelper StaticFiles { get; }

        /// <summary>
        /// Helper for accessing configuration settings from a view
        /// </summary>
        public ISettingsViewHelper Settings { get; }

        /// <summary>
        /// Helpers for accessing information about the currently logged in user
        /// </summary>
        public ICurrentUserViewHelper CurrentUser { get; }

        /// <summary>
        /// Helper for working with javascript from .net code
        /// </summary>
        public IJavascriptViewHelper Js { get; }

        /// <summary>
        /// Helper for sanitizing html before it output to the page. You'd typically
        /// want to use this when rendering out user inputted data which may be 
        /// vulnerable to XSS attacks.
        /// </summary>
        public IHtmlSanitizerHelper Sanitizer { get; }

        /// <summary>
        /// Miscellaneous helper functions to make working work html views easier.
        /// </summary>
        public ICofoundryHtmlHelper Html { get; }
    }
}
