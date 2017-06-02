using Cofoundry.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Locates and renders razor view files to strings.
    /// </summary>
    public interface IRazorViewRenderer
    {
        /// <summary>
        /// Renders a view to a string using an existing ActionContext.
        /// </summary>
        /// <param name="actionContext">An existing action context to copy data from when rendering the new view.</param>
        /// <param name="viewName">The name of the view to locate. Supports view location and full paths.</param>
        Task<string> RenderViewAsync(
            ActionContext actionContext,
            string viewName
            );

        /// <summary>
        /// Renders a view to a string using an existing ActionContext.
        /// </summary>
        /// <param name="actionContext">An existing action context to copy data from when rendering the new view.</param>
        /// <param name="viewName">The name of the view to locate. Supports view location and full paths.</param>
        /// <param name="model">The data model to render the view with.</param>
        Task<string> RenderViewAsync(
            ActionContext actionContext,
            string viewName,
            object model
            );

        /// <summary>
        /// Renders a view to a string using an existing ViewContext.
        /// </summary>
        /// <param name="viewContext">An existing view context to copy data from when rendering the new view.</param>
        /// <param name="viewName">The name of the view to locate. Supports view location and full paths.</param>
        Task<string> RenderViewAsync(
            ViewContext viewContext,
            string viewName
            );

        /// <summary>
        /// Renders a view to a string using an existing ViewContext.
        /// </summary>
        /// <param name="viewContext">An existing view context to copy data from when rendering the new view.</param>
        /// <param name="viewName">The name of the view to locate. Supports view location and full paths.</param>
        /// <param name="model">The data model to render the view with.</param>
        Task<string> RenderViewAsync(
            ViewContext viewContext,
            string viewName,
            object model
            );
    }
}
