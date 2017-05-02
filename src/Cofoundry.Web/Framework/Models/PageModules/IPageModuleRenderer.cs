using System;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Controls rendering of page modules, rendering the razor templates out to 
    /// a string
    /// </summary>
    public interface IPageModuleRenderer
    {
        /// <summary>
        /// Renders a page module by finding the template and applying the specified model to it
        /// </summary>
        /// <param name="controllerContext">ControllerContext is required so we can render the razor view</param>
        /// <param name="pageViewModel">The view model for the page being rendered</param>
        /// <param name="moduleViewModel">The view model for the module being rendered</param>
        /// <returns>The rednered module html</returns>
        Task<string> RenderModuleAsync(
            ViewContext controllerContext, 
            IEditablePageViewModel pageViewModel, 
            IEntityVersionPageModuleRenderDetails moduleViewModel
            );

        /// <summary>
        /// Renders an default placeholder element for use when a module has not yet been added to a section.
        /// </summary>
        /// <param name="minHeight">the min-height to apply to the element.</param>
        string RenderPlaceholderModule(int? minHeight = null);
    }
}
