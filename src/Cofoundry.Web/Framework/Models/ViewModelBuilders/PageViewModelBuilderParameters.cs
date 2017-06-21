using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Parameters for mapping an IPageViewModel using an
    /// IPageViewModelMapper implementation.
    /// </summary>
    /// <remarks>
    /// It's possible for the mapper to be overriden and customized so 
    /// this parameter class helps future proof the design in case we 
    /// want to add more parameters in the future.
    /// </remarks>
    public class PageViewModelBuilderParameters
    {
        /// <summary>
        /// Parameters for mapping an IPageViewModel using an
        /// IPageViewModelBuilder implementation.
        /// </summary>
        /// <param name="pageModel">The page data to include in the view model.</param>
        /// <param name="visualEditorMode">The view mode requested by the visual editor or a default value.</param>
        public PageViewModelBuilderParameters(
               PageRenderDetails pageModel,
               VisualEditorMode visualEditorMode
               )
        {
            PageModel = pageModel;
            VisualEditorMode = visualEditorMode;
        }

        /// <summary>
        /// The page data to include in the view model.
        /// </summary>
        public PageRenderDetails PageModel { get; set; }

        /// <summary>
        /// The view mode requested by the visual editor (if available).
        /// </summary>
        public VisualEditorMode VisualEditorMode { get; set; }
    }
}