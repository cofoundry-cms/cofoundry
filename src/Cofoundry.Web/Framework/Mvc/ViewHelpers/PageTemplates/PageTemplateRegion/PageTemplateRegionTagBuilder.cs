using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    /// <summary>
    /// Fluent builder for defining a page template region configuration using method chaining
    /// </summary>
    public class PageTemplateRegionTagBuilder : IPageTemplateRegionTagBuilder
    {
        #region constructor

        private readonly IEditablePageViewModel _pageViewModel;
        private readonly ViewContext _viewContext;
        private readonly IPageBlockRenderer _blockRenderer;
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockTypeDataModelTypeFactory;
        private readonly IPageBlockTypeFileNameFormatter _pageBlockTypeTypeFileNameFormatter;

        public PageTemplateRegionTagBuilder(
            IPageBlockRenderer blockRenderer,
            IPageBlockTypeDataModelTypeFactory pageBlockTypeDataModelTypeFactory,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter,
            ViewContext viewContext,
            IEditablePageViewModel pageViewModel, 
            string regionName
            )
        {
            if (regionName == null) throw new ArgumentNullException(nameof(regionName));
            if (string.IsNullOrWhiteSpace(regionName)) throw new ArgumentEmptyException(nameof(regionName));
            if (pageViewModel == null) throw new ArgumentNullException(nameof(pageViewModel));

            _blockRenderer = blockRenderer;
            _pageBlockTypeDataModelTypeFactory = pageBlockTypeDataModelTypeFactory;
            _pageBlockTypeTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
            _regionName = regionName;
            _pageViewModel = pageViewModel;
            _viewContext = viewContext;
        }

        #endregion

        #region state properties

        private string _output = null;
        private string _regionName;
        private string _wrappingTagName = null;
        private bool _allowMultipleBlocks = false;
        private int? _emptyContentMinHeight = null;
        private Dictionary<string, string> _additonalHtmlAttributes = null;
        private Dictionary<string, object> _permittedBlocks = new Dictionary<string, object>();

        #endregion

        #region public chaining methods

        /// <summary>
        /// By default only a single block is allowed per region. This method configures the
        /// region to allow an unlimited number of blocks to be added
        /// </summary>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        public IPageTemplateRegionTagBuilder AllowMultipleBlocks()
        {
            _allowMultipleBlocks = true;
            return this;
        }

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
        public IPageTemplateRegionTagBuilder WrapWithTag(string tagName, object htmlAttributes = null)
        {
            if (tagName == null) throw new ArgumentNullException(nameof(tagName));
            if (string.IsNullOrWhiteSpace(tagName)) throw new ArgumentEmptyException(nameof(tagName));

            _wrappingTagName = tagName;
            _additonalHtmlAttributes = TemplateRegionTagBuilderHelper.ParseHtmlAttributesFromAnonymousObject(htmlAttributes);

            return this;
        }

        /// <summary>
        /// Sets the default height of the region when it contains no
        /// blocks and the page is in edit mode. This makes it easier to visualize
        /// and select the region when a template is first created.
        /// </summary>
        /// <param name="minHeight">The minimum height in pixels</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        public IPageTemplateRegionTagBuilder EmptyContentMinHeight(int minHeight)
        {
            _emptyContentMinHeight = minHeight;
            return this;
        }

        /// <summary>
        /// Permits the block type associated with the specified data model type
        /// to be selected and used in this page region. By default all block types are 
        /// available but using this method changes the block selection to an 'opt-in'
        /// configuration.
        /// </summary>
        /// <typeparam name="TBlockType">The data model type of the block type to register</typeparam>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        public IPageTemplateRegionTagBuilder AllowBlockType<TBlockType>() where TBlockType : IPageBlockTypeDataModel
        {
            AddBlockToAllowedTypes(typeof(TBlockType).Name);
            return this;
        }

        /// <summary>
        /// Permits the specified block type to be selected and used in this page 
        /// region. By default all blocks are available but using this method changes 
        /// the block selection to an 'opt-in' configuration.
        /// </summary>
        /// <param name="blockTypeName">The name of the block type to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        public IPageTemplateRegionTagBuilder AllowBlockType(string blockTypeName)
        {
            AddBlockToAllowedTypes(blockTypeName);

            return this;
        }

        /// <summary>
        /// Permits the specified block type to be selected and used in this page 
        /// region. By default all block types are available but using this method changes 
        /// the block selection to an 'opt-in' configuration.
        /// configuration.
        /// </summary>
        /// <param name="blockTypeNames">The names of the block type to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>IPageTemplateRegionTagBuilder for method chaining</returns>
        public IPageTemplateRegionTagBuilder AllowBlockTypes(params string[] blockTypeNames)
        {
            foreach (var blockTypeName in blockTypeNames)
            {
                AddBlockToAllowedTypes(blockTypeName);
            }
            return this;
        }

        private void AddBlockToAllowedTypes(string blockTypeName)
        {
            // Get the file name if we haven't already got it, e.g. we could have the file name or 
            // full type name passed into here (we shouldn't be fussy)
            var fileName = _pageBlockTypeTypeFileNameFormatter.FormatFromDataModelName(blockTypeName);

            // Validate the model type, will throw exception if not implemented
            var blockType = _pageBlockTypeDataModelTypeFactory.CreateByPageBlockTypeFileName(fileName);

            // Make sure we have the correct name casing
            var formattedBlockypeName = _pageBlockTypeTypeFileNameFormatter.FormatFromDataModelType(blockType);

            _permittedBlocks.Add(formattedBlockypeName, null);
        }

        #endregion

        #region rendering

        /// <summary>
        /// This method must be called at the end of the region definition to build and render the
        /// region.
        /// </summary>
        public async Task<IPageTemplateRegionTagBuilder> InvokeAsync()
        {
            var pageRegion = _pageViewModel
                .Page
                .Regions
                .SingleOrDefault(s => s.Name == _regionName);

            if (pageRegion != null)
            {
                _output = await RenderRegion(pageRegion);
            }
            else
            {
                var msg = "WARNING: The page region '" + _regionName + "' cannot be found in the database";

                Debug.Assert(pageRegion != null, msg);
                if (_pageViewModel.Page.WorkFlowStatus != WorkFlowStatus.Published)
                {
                    _output = "<!-- " + msg + " -->";
                }
            }

            return this;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            Debug.Assert(_output != null, $"Template region '{ _regionName }' definition does not call { nameof(InvokeAsync)}().");

            if (_output != null)
            {
                writer.Write(_output);
            }
        }

        private async Task<string> RenderRegion(PageRegionRenderDetails pageRegion)
        {
            string blocksHtml = string.Empty;
            
            // No _permittedBlocks means any is allowed. 
            var renderingTasks = pageRegion
                .Blocks
                .Where(m => _permittedBlocks.Count == 0 || _permittedBlocks.ContainsKey(m.BlockType.FileName))
                .Select(m => _blockRenderer.RenderBlockAsync(_viewContext, _pageViewModel, m));

            var blockHtmlParts = await Task.WhenAll(renderingTasks);

            if (blockHtmlParts.Any())
            {
                if (!_allowMultipleBlocks)
                {
                    // If for some reason another block has been added in error, make sure we only display one.
                    blocksHtml = blockHtmlParts.Last();
                }
                else
                {
                    blocksHtml = string.Join(string.Empty, blockHtmlParts);
                }
            }
            else if (!_allowMultipleBlocks)
            {
                // If there are no blocks and this is a single block region
                // add a placeholder element so we always have a menu
                blocksHtml = _blockRenderer.RenderPlaceholderBlock(_emptyContentMinHeight);
            }

            // If we're not in edit mode just return the blocks.
            if (!_pageViewModel.IsPageEditMode)
            {
                if (_wrappingTagName != null)
                {
                    return TemplateRegionTagBuilderHelper.WrapInTag(
                        blocksHtml,
                        _wrappingTagName,
                        _allowMultipleBlocks,
                        _additonalHtmlAttributes
                        );
                }

                return blocksHtml;
            }

            var attrs = new Dictionary<string, string>();
            attrs.Add("data-cms-page-template-region-id", pageRegion.PageTemplateRegionId.ToString());
            attrs.Add("data-cms-page-region-name", pageRegion.Name);
            attrs.Add("data-cms-page-region", string.Empty);
            attrs.Add("class", "cofoundry__sv-region");

            if (_permittedBlocks.Any())
            {
                var permittedBlockTypes = _permittedBlocks.Select(m => m.Key);
                attrs.Add("data-cms-page-region-permitted-block-types", string.Join(",", permittedBlockTypes));
            }

            if (_allowMultipleBlocks)
            {
                attrs.Add("data-cms-multi-block", "true");

                if (_emptyContentMinHeight.HasValue)
                {
                    attrs.Add("style", "min-height:" + _emptyContentMinHeight + "px");
                }
            }

            return TemplateRegionTagBuilderHelper.WrapInTag(
                blocksHtml,
                _wrappingTagName,
                _allowMultipleBlocks,
                _additonalHtmlAttributes,
                attrs
                );
        }

        #endregion
    }
}
