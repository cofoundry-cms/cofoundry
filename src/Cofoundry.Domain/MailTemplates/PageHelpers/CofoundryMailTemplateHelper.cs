using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
    public class CofoundryMailTemplateHelper : ICofoundryMailTemplateHelper
    {
        public CofoundryMailTemplateHelper(
            IContentRouteLibrary contentRouteLibrary,
            IHtmlSanitizerHelper htmlSanitizerHelper
            )
        {
            Routing = contentRouteLibrary;
            Sanitizer = htmlSanitizerHelper;
        }

        /// <summary>
        /// Helpers for generating links to Cofoundry content
        /// </summary>
        public IContentRouteLibrary Routing { get; private set; }

        /// <summary>
        /// Helper for sanitizing html before it output to the page. You'd typically
        /// want to use this when rendering out user inputted data which may be 
        /// vulnerable to XSS attacks.
        /// </summary>
        public IHtmlSanitizerHelper Sanitizer { get; private set; }
    }
}
