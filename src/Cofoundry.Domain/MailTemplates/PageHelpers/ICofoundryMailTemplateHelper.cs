using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Main helper for Cofoundry functionality on pages with a 
    /// model defined. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid poluting 
    /// the global namespace.
    /// </summary>
    public interface ICofoundryMailTemplateHelper
    {
        /// <summary>
        /// Helpers for generating links to Cofoundry content
        /// </summary>
        IContentRouteLibrary Routing { get; }

        /// <summary>
        /// Helper for sanitizing html before it output to the page. You'd typically
        /// want to use this when rendering out user inputted data which may be 
        /// vulnerable to XSS attacks.
        /// </summary>
        IHtmlSanitizerHelper Sanitizer { get; }
    }
}
