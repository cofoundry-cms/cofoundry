using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
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
    public class CofoundryMailTemplatePageHelper<T>
    {
        public CofoundryMailTemplatePageHelper(ViewContext viewContext, T model)
        {
            var serviceProvider = viewContext.HttpContext.RequestServices;

            Routing = serviceProvider.GetRequiredService<IContentRouteLibrary>();
            Sanitizer = serviceProvider.GetRequiredService<IHtmlSanitizerHelper>();

            Model = model;
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

        /// <summary>
        /// The view model associated with the page this helper is contained in
        /// </summary>
        public T Model { get; private set; }
    }
}
