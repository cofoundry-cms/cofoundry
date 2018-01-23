using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main helper for Cofoundry functionality on pages with a 
    /// model defined. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid poluting 
    /// the global namespace.
    /// </summary>
    public class CofoundryPageHelper<TModel> : ICofoundryHelper<TModel>, IViewContextAware
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

        public virtual void Contextualize(ViewContext viewContext)
        {
            if (viewContext.ViewData.Model is TModel model)
            {
                Model = model;
            }
            else if (viewContext.ViewData.Model != null)
            {
                throw new Exception($"The view model type '{viewContext.ViewData.Model?.GetType().Name }' does not match the generic type parameter '{typeof(TModel).Name}'");
            }
        }

        /// <summary>
        /// The view model associated with the page this helper is contained in
        /// </summary>
        public TModel Model { get; private set; }

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
