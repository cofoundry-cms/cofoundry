using System;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Controls rendering of page blocks, rendering the razor templates out to 
    /// a string.
    /// </summary>
    public interface IPageBlockRenderer
    {
        /// <summary>
        /// Renders a page block by finding the template and applying the specified model to it
        /// </summary>
        /// <param name="viewContext">ViewContext is required so we can render the razor view.</param>
        /// <param name="pageViewModel">The view model for the page being rendered.</param>
        /// <param name="blockViewModel">The view model for the module being rendered.</param>
        /// <returns>The rendered module html.</returns>
        Task<string> RenderBlockAsync(
            ViewContext controllerContext, 
            IEditablePageViewModel pageViewModel, 
            IEntityVersionPageBlockRenderDetails blockViewModel
            );

        /// <summary>
        /// Renders an default placeholder element for use when a page block has not yet been added to a region.
        /// </summary>
        /// <param name="minHeight">the min-height to apply to the element.</param>
        string RenderPlaceholderBlock(int? minHeight = null);
    }
}
