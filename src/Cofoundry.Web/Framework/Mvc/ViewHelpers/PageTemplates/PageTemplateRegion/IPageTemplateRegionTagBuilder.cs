using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Fluent builder for defining a page template region configuration using method chaining.
    /// </summary>
    public interface IPageTemplateRegionTagBuilder : IHtmlContent
    {
        /// <summary>
        /// Permits the block type associated with the specified data model type
        /// to be selected and used in this page region. By default all block types are 
        /// available but using this method changes the block selection to an 'opt-in'
        /// configuration.
        /// </summary>
        /// <typeparam name="TBlockType">The data model type of the block type to register</typeparam>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        IPageTemplateRegionTagBuilder AllowBlockType<TBlockType>() where TBlockType : IPageBlockTypeDataModel;

        /// <summary>
        /// Permits the specified block type to be selected and used in this page 
        /// region. By default all blocks are available but using this method changes 
        /// the block selection to an 'opt-in' configuration.
        /// </summary>
        /// <param name="blockTypeName">The name of the block type to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        IPageTemplateRegionTagBuilder AllowBlockType(string blockTypeName);

        /// <summary>
        /// Permits the specified block type to be selected and used in this page 
        /// region. By default all block types are available but using this method changes 
        /// the block selection to an 'opt-in' configuration.
        /// configuration.
        /// </summary>
        /// <param name="blockTypeNames">The names of the block type to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        IPageTemplateRegionTagBuilder AllowBlockTypes(params string[] blockTypeNames);

        /// <summary>
        /// By default only a single block is allowed per region. This method configures the
        /// region to allow an unlimited number of blocks to be added
        /// </summary>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        IPageTemplateRegionTagBuilder AllowMultipleBlocks();

        /// <summary>
        /// Sets the default height of the region when it contains no
        /// blocks and the page is in edit mode. This makes it easier to visualize
        /// and select the region when a template is first created.
        /// </summary>
        /// <param name="minHeight">The minimum height in pixels</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        IPageTemplateRegionTagBuilder EmptyContentMinHeight(int minHeight);

        /// <summary>
        /// Wraps blocks within the region in a specific html tag. Each region
        /// needs to have a single container node in page edit mode and by default a div
        /// element is used if a single container cannot be found in the rendered output.
        /// Use this setting to control this tag. If a tag is specified here the output
        /// will always be wrapped irrespecitive of whether the page is in edit mode to maintain
        /// consistency.
        /// </summary>
        /// <param name="tagName">Name of the element to wrap the output in e.g. div, p, header</param>
        /// <param name="htmlAttributes">Html attributes to apply to the wrapping tag.</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        IPageTemplateRegionTagBuilder WrapWithTag(string tagName, object htmlAttributes = null);

        /// <summary>
        /// This method must be called at the end of the region definition to build and render the
        /// region.
        /// </summary>
        Task<IPageTemplateRegionTagBuilder> InvokeAsync();
    }
}
