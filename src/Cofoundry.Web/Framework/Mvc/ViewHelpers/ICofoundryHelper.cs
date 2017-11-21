using Cofoundry.Domain;
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
    public interface ICofoundryHelper
    {
        /// <summary>
        /// Helpers for generating links to Cofoundry content
        /// </summary>
        IContentRouteLibrary Routing { get; }

        /// <summary>
        /// Helper for resolving urls to static files.
        /// </summary>
        IStaticFileViewHelper StaticFiles { get; }

        /// <summary>
        /// Helper for accessing configuration settings from a view
        /// </summary>
        ISettingsViewHelper Settings { get; }

        /// <summary>
        /// Helpers for accessing information about the currently logged in user
        /// </summary>
        ICurrentUserViewHelper CurrentUser { get; }

        /// <summary>
        /// Helper for working with javascript from .net code
        /// </summary>
        IJavascriptViewHelper Js { get; }

        /// <summary>
        /// Helper for sanitizing html before it output to the page. You'd typically
        /// want to use this when rendering out user inputted data which may be 
        /// vulnerable to XSS attacks.
        /// </summary>
        IHtmlSanitizerHelper Sanitizer { get; }

        /// <summary>
        /// Miscellaneous helper functions to make working work html views easier.
        /// </summary>
        ICofoundryHtmlHelper Html { get; }
    }
}
