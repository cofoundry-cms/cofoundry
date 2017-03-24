using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// Parameters for mapping an ICustomEntityDetailsPageViewModel using an
    /// IPageViewModelBuilder implementation.
    /// </summary>
    /// <remarks>
    /// It's possible for the mapper to be overriden and customized so 
    /// this parameter class helps future proof the design in case we 
    /// want to add more parameters in the future.
    /// </remarks>
    public class CustomEntityDetailsPageViewModelBuilderParameters : PageViewModelBuilderParameters
    {
        /// <summary>
        /// Parameters for mapping an ICustomEntityDetailsPageViewModel using an
        /// IPageViewModelMapper implementation.
        /// </summary>
        /// <param name="pageModel">The page data to include in the view model.</param>
        /// <param name="visualEditorMode">The view mode requested by the visual editor or a default value.</param>
        /// <param name="customEntityModel">he custom entity model data to use in the mapping.</param>
        public CustomEntityDetailsPageViewModelBuilderParameters(
               PageRenderDetails page,
               VisualEditorMode visualEditorMode,
               CustomEntityRenderDetails customEntityModel
               )
            : base(page, visualEditorMode)
        {
            CustomEntityModel = customEntityModel;
        }

        /// <summary>
        /// The custom entity model data to use in the mapping.
        /// </summary>
        public CustomEntityRenderDetails CustomEntityModel { get; set; }
    }
}