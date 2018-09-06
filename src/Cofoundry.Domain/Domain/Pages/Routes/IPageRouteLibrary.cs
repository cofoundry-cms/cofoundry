using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Route library for pages
    /// </summary>
    public interface IPageRouteLibrary
    {
        /// <summary>
        /// Simple but less efficient way of getting a page url if you only know 
        /// the id. Use the overload accepting an IPageRoute if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        Task<string> PageAsync(int? pageId);

        /// <summary>
        /// Gets the full (relative) url of a page
        /// </summary>
        string Page(IPageRoute route);

        /// <summary>
        /// Gets the full (relative) url of a custom entity details page
        /// </summary>
        string Page(ICustomEntityRoutable customEntity);

        /// <summary>
        /// Gets the url for a page, formatted with specific visual editor 
        /// parameters. Note that this method does not validate permissions
        /// in any way, it simply formats the route correctly.
        /// </summary>
        /// <param name="route">The page to link to.</param>
        /// <param name="visualEditorMode">
        /// The mode to set the visual editor to. Note that this method cannot be
        /// used for VisualEditorMode.SpecificVersion and will throw an exception if
        /// you try. To get the url for a specific version, you need to use the overload
        /// accepting an IVersionRoute parameter.
        /// </param>
        /// <param name="isEditingCustomEntity">
        /// For custom entity pages, this option indicates whether the editing context 
        /// should be the custom entity rather than the (default) page.
        /// </param>
        string VisualEditor(
            PageRoutingInfo route,
            VisualEditorMode visualEditorMode,
            bool isEditingCustomEntity = false
            );

        /// <summary>
        /// Gets the url for a page at a specific page or custom entity version, loaded inside the 
        /// visual editor. Note that this method does not validate permissions in any way, it simply 
        /// formats the route correctly.
        /// </summary>
        /// <param name="route">The page to link to.</param>
        /// <param name="versionRoute">The version of the page or custom entity to link to.</param>
        string VisualEditor(
            PageRoutingInfo route,
            IVersionRoute versionRoute
            );
    }
}
